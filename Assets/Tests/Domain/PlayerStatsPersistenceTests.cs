using Domain.Models;
using Domain.Services;
using NUnit.Framework;

namespace Domain.Tests
{
    [TestFixture]
    public class PlayerStatsPersistenceTests
    {
        private MemoryStorage _storage;
        private PlayerService _player;
        private PlayerStatsPersistence _persistence;

        [SetUp]
        public void SetUp()
        {
            var arena = new RectangleArena(3, 4, new RandomSource(0));
            var state = new GameStateService();
            var time = TestClock.Create(state);
            _player = new PlayerService(new Player(1, new GridPosition(1, 1)), arena, new AttackService(time, state), time);
            _storage = new MemoryStorage();
            _persistence = new PlayerStatsPersistence(_player, _storage);
        }

        [Test]
        public void HandleExitGameplay_SavesHighScoreAndGold()
        {
            _player.GivePlayerScore(7);
            _player.GivePlayerGold(3);

            _persistence.HandleExitGameplay();

            Assert.AreEqual(7, _storage.Load<int>(StorageKeys.Highscore.ToString()));
            Assert.AreEqual(3, _storage.Load<int>(StorageKeys.Gold.ToString()));
        }

        [Test]
        public void HandleStartGame_LoadsSavedHighScoreAndGold()
        {
            _storage.Save(StorageKeys.Highscore.ToString(), 12);
            _storage.Save(StorageKeys.Gold.ToString(), 5);

            _persistence.HandleStartGame();

            Assert.AreEqual(12, _player.HighScore);
            Assert.AreEqual(5, _player.Gold);
        }

        [Test]
        public void HandleStartGame_WithNothingSaved_KeepsCurrentStats()
        {
            _player.HighScore = 4;
            _player.Gold = 2;

            _persistence.HandleStartGame();

            Assert.AreEqual(4, _player.HighScore);
            Assert.AreEqual(2, _player.Gold);
        }

        [Test]
        public void SavedStats_SurviveARoundTrip()
        {
            _player.GivePlayerScore(9);
            _player.GivePlayerGold(6);
            _persistence.HandleExitGameplay();

            _player.HighScore = 0;
            _player.Gold = 0;
            _persistence.HandleStartGame();

            Assert.AreEqual(9, _player.HighScore);
            Assert.AreEqual(6, _player.Gold);
        }
    }
}
