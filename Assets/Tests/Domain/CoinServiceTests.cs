using Domain.Models;
using Domain.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Domain.Tests
{
    [TestFixture]
    public class CoinServiceTests
    {
        private readonly GridPosition _start = new GridPosition(1, 1);
        private RectangleArena _arena;
        private GameStateService _state;
        private PlayerService _player;
        private RecordingLogger _logger;
        private CoinService _coins;
        private List<Coin> _created;

        [SetUp]
        public void SetUp()
        {
            _arena = new RectangleArena(3, 4, new RandomSource(0));
            _state = new GameStateService();
            var time = TestClock.Create(_state);
            _player = new PlayerService(new Player(1, _start), _arena, new AttackService(time, _state), time);
            _logger = new RecordingLogger();
            _coins = new CoinService(_logger, _arena, _player, _state, new RandomSource(0));
            _created = new List<Coin>();
            _coins.OnCoinCreated += _created.Add;
            _state.TransitionTo(GameState.Gameplay);
        }

        private static int ManhattanDistance(GridPosition a, GridPosition b)
        {
            return Math.Abs(a.GridX - b.GridX) + Math.Abs(a.GridY - b.GridY);
        }

        [Test]
        public void CreateCoin_PlacesOneCoinOnTheArena()
        {
            _coins.CreateCoinAtRandomPosition(0);

            Assert.AreEqual(1, _coins.CoinCount);
            Assert.AreEqual(1, _created.Count);
            Assert.IsTrue(_arena.IsInBounds(_created[0].Position));
        }

        [Test]
        public void CreateCoin_KeepsTheMinimumDistanceFromThePlayer()
        {
            for (int i = 0; i < 50; i++) _coins.CreateCoinAtRandomPosition(3);

            foreach (var coin in _created)
                Assert.GreaterOrEqual(ManhattanDistance(coin.Position, _start), 3, $"coin at {coin.Position}");
        }

        [Test]
        public void CreateCoin_WithNoTileFarEnough_WarnsAndPlacesNothing()
        {
            _coins.CreateCoinAtRandomPosition(100);

            Assert.AreEqual(0, _coins.CoinCount);
            CollectionAssert.IsEmpty(_created);
            Assert.AreEqual(1, _logger.Warnings.Count);
        }

        [Test]
        public void SteppingOnTheCoin_CollectsItForOneGold()
        {
            _coins.CreateCoinAtRandomPosition(0);
            var coin = _created[0];
            var removed = new List<Coin>();
            _coins.OnCoinRemoved += removed.Add;

            _player.MovePlayer(coin.Position);

            Assert.AreEqual(0, _coins.CoinCount);
            Assert.AreEqual(1, _player.Gold);
            CollectionAssert.AreEqual(new[] { coin }, removed);
        }

        [Test]
        public void SteppingElsewhere_LeavesTheCoin()
        {
            // Two steps away at least, so the tile one step up never holds the coin.
            _coins.CreateCoinAtRandomPosition(2);

            _player.MovePlayer(GridDirection.Up);

            Assert.AreEqual(1, _coins.CoinCount);
            Assert.AreEqual(0, _player.Gold);
        }

        [Test]
        public void LeavingGameplay_RemovesTheCoin()
        {
            _coins.CreateCoinAtRandomPosition(0);
            var coin = _created[0];
            var removed = new List<Coin>();
            _coins.OnCoinRemoved += removed.Add;

            _state.TransitionTo(GameState.MainMenu);

            Assert.AreEqual(0, _coins.CoinCount);
            CollectionAssert.AreEqual(new[] { coin }, removed, "the removed coin is reported, so its visual can go");
        }

        [Test]
        public void CreateCoin_WithTheSameSeed_PicksTheSameTiles()
        {
            List<GridPosition> Positions(int seed)
            {
                var state = new GameStateService();
                var arena = new RectangleArena(3, 4, new RandomSource(seed));
                var time = TestClock.Create(state);
                var player = new PlayerService(new Player(1, _start), arena, new AttackService(time, state), time);
                var coins = new CoinService(new RecordingLogger(), arena, player, state, new RandomSource(seed));
                var created = new List<GridPosition>();
                coins.OnCoinCreated += coin => created.Add(coin.Position);
                state.TransitionTo(GameState.Gameplay);
                for (int i = 0; i < 20; i++) coins.CreateCoinAtRandomPosition(0);
                return created;
            }

            var first = Positions(5);
            Assert.AreEqual(20, first.Count);
            CollectionAssert.AreEqual(first, Positions(5));
        }
    }
}
