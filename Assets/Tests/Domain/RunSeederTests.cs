using Domain.Models;
using Domain.Services;
using NUnit.Framework;

namespace Domain.Tests
{
    [TestFixture]
    public class RunSeederTests
    {
        private GameStateService _state;
        private RandomSource _runRandom;
        private RecordingLogger _logger;

        [SetUp]
        public void SetUp()
        {
            _state = new GameStateService();
            _runRandom = new RandomSource(1);
            _logger = new RecordingLogger();
        }

        [Test]
        public void StartingARun_ReseedsWithTheFixedSeed()
        {
            new RunSeeder(_state, _runRandom, _logger, 42);
            _runRandom.Range(0, 100);

            _state.TransitionTo(GameState.Gameplay);

            Assert.AreEqual(42, _runRandom.Seed);
            Assert.AreEqual(new RandomSource(42).Range(0, 100), _runRandom.Range(0, 100));
        }

        [Test]
        public void EachRun_RestartsTheSequence()
        {
            new RunSeeder(_state, _runRandom, _logger, 42);

            _state.TransitionTo(GameState.Gameplay);
            int firstRun = _runRandom.Range(0, 1000);
            _state.TransitionTo(GameState.MainMenu);
            _state.TransitionTo(GameState.Gameplay);

            Assert.AreEqual(firstRun, _runRandom.Range(0, 1000));
        }

        [Test]
        public void StartingARun_LogsTheSeed()
        {
            new RunSeeder(_state, _runRandom, _logger, 42);

            _state.TransitionTo(GameState.Gameplay);

            Assert.That(_logger.Infos, Has.Some.Contains("42"));
        }

        [Test]
        public void WithoutAFixedSeed_EachRunGetsANewSeed()
        {
            new RunSeeder(_state, _runRandom, _logger);

            _state.TransitionTo(GameState.Gameplay);
            int first = _runRandom.Seed;
            _state.TransitionTo(GameState.MainMenu);
            _state.TransitionTo(GameState.Gameplay);

            Assert.AreNotEqual(first, _runRandom.Seed);
        }

        [Test]
        public void LeavingARun_DoesNotReseed()
        {
            new RunSeeder(_state, _runRandom, _logger, 42);
            _state.TransitionTo(GameState.Gameplay);
            var expected = new RandomSource(42);
            expected.Range(0, 100);
            _runRandom.Range(0, 100);

            _state.TransitionTo(GameState.MainMenu);

            Assert.AreEqual(expected.Range(0, 100), _runRandom.Range(0, 100));
        }
    }
}
