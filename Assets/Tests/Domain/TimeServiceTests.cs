using Domain.Models;
using Domain.Services;
using NUnit.Framework;
using System.Collections.Generic;

namespace Domain.Tests
{
    [TestFixture]
    public class TimeServiceTests
    {
        private GameStateService _state;
        private TimeService _time;
        private List<float> _ticks;

        [SetUp]
        public void SetUp()
        {
            _state = new GameStateService();
            _time = new TimeService(_state);
            _ticks = new List<float>();
            _time.OnTick += dt => _ticks.Add(dt);
        }

        [Test]
        public void Tick_PassesDeltaTimeOn()
        {
            _time.Tick(0.25f);

            CollectionAssert.AreEqual(new[] { 0.25f }, _ticks);
        }

        [Test]
        public void Tick_WhilePaused_DoesNothing()
        {
            _time.Pause();
            _time.Tick(0.25f);

            Assert.IsTrue(_time.IsPaused);
            CollectionAssert.IsEmpty(_ticks);
        }

        [Test]
        public void Resume_LetsTicksThroughAgain()
        {
            _time.Pause();
            _time.Resume();
            _time.Tick(0.25f);

            Assert.IsFalse(_time.IsPaused);
            CollectionAssert.AreEqual(new[] { 0.25f }, _ticks);
        }

        [Test]
        public void EnteringGameplay_Resumes()
        {
            _time.Pause();
            _state.TransitionTo(GameState.Gameplay);

            Assert.IsFalse(_time.IsPaused);
        }

        [Test]
        public void ReturningToMainMenu_Pauses()
        {
            _state.TransitionTo(GameState.Gameplay);
            _state.TransitionTo(GameState.MainMenu);

            Assert.IsTrue(_time.IsPaused);
        }
    }
}
