using Domain.Models;
using Domain.Services;
using NUnit.Framework;
using System.Collections.Generic;

namespace Domain.Tests
{
    [TestFixture]
    public class PlayerServiceTests
    {
        private readonly GridPosition _start = new GridPosition(1, 1);
        private RectangleArena _arena;
        private GameStateService _state;
        private TimeService _time;
        private AttackService _attacks;
        private PlayerService _player;

        [SetUp]
        public void SetUp()
        {
            // Same wiring order as GameBootstrap.Awake, same arena size as the scene.
            _arena = new RectangleArena(3, 4, new RandomSource(0));
            _state = new GameStateService();
            _time = TestClock.Create(_state);
            _attacks = new AttackService(_time, _state);
            _player = new PlayerService(new Player(1, _start), _arena, _attacks, _time);
            _state.TransitionTo(GameState.Gameplay);
        }

        // Each stage lasts one second, so the hammer's impact runs from 2s to 3s.
        private void SpawnHammerAt(int x, int y)
        {
            _attacks.SpawnAttack(new HammerAttack(new GridPosition(x, y), HorizontalDirection.LeftToRight,
                new AttackTiming(1f, 1f, 1f, 1f), _arena));
        }

        [Test]
        public void MovePlayer_InsideTheArena_MovesAndReportsTheMove()
        {
            var moves = new List<(GridPosition from, GridPosition to)>();
            _player.OnPlayerMove += (from, to) => moves.Add((from, to));

            _player.MovePlayer(GridDirection.Up);

            Assert.AreEqual(new GridPosition(1, 2), _player.GetPlayerGridPosition());
            Assert.AreEqual(1, moves.Count);
            Assert.AreEqual(_start, moves[0].from);
            Assert.AreEqual(new GridPosition(1, 2), moves[0].to);
        }

        [Test]
        public void MovePlayer_OffTheArena_StaysPut()
        {
            int moves = 0;
            _player.OnPlayerMove += (from, to) => moves++;
            _player.MovePlayer(new GridPosition(0, 1));

            _player.MovePlayer(GridDirection.Left);

            Assert.AreEqual(new GridPosition(0, 1), _player.GetPlayerGridPosition());
            Assert.AreEqual(1, moves);
        }

        [Test]
        public void StandingInAnImpact_HitsAndKills()
        {
            int hits = 0, deaths = 0;
            _player.OnPlayerAttackHit += () => hits++;
            _player.OnPlayerDied += () => deaths++;
            SpawnHammerAt(1, 1);

            _time.Advance(1f);
            _time.Advance(1f);
            _time.Advance(0.1f);

            Assert.AreEqual(1, hits);
            Assert.AreEqual(1, deaths);
            Assert.IsFalse(_player.IsPlayerAlive());
            Assert.AreEqual(0, _player.GetPlayerHealth());
        }

        [Test]
        public void StandingClearOfAnImpact_IsNotHit()
        {
            int hits = 0;
            _player.OnPlayerAttackHit += () => hits++;
            SpawnHammerAt(0, 3);

            for (int i = 0; i < 4; i++) _time.Advance(1f);

            Assert.AreEqual(0, hits);
            Assert.IsTrue(_player.IsPlayerAlive());
        }

        [Test]
        public void SteppingIntoAnImpact_Hits()
        {
            int hits = 0;
            _player.OnPlayerAttackHit += () => hits++;
            SpawnHammerAt(1, 3);
            _time.Advance(1f);
            _time.Advance(1f);
            _time.Advance(0.1f);
            Assume.That(hits, Is.EqualTo(0));

            _player.MovePlayer(GridDirection.Up);

            Assert.AreEqual(1, hits);
        }

        [Test]
        public void CompletedAttack_AwardsOnePoint()
        {
            SpawnHammerAt(0, 3);

            for (int i = 0; i < 4; i++) _time.Advance(1f);

            Assert.AreEqual(1, _player.Score);
        }

        [Test]
        public void HighScore_FollowsScore_WhenScorePassesIt()
        {
            var highScores = new List<int>();
            _player.OnPlayerHighScoreChange += highScores.Add;
            _player.HighScore = 2;

            _player.GivePlayerScore(3);

            Assert.AreEqual(3, _player.Score);
            Assert.AreEqual(3, _player.HighScore);
            CollectionAssert.AreEqual(new[] { 2, 3 }, highScores);
        }

        [Test]
        public void HighScore_StaysPut_WhileScoreIsBelowIt()
        {
            _player.HighScore = 5;

            _player.GivePlayerScore(3);

            Assert.AreEqual(5, _player.HighScore);
        }

        [Test]
        public void GivePlayerGold_AddsGold_AndReportsIt()
        {
            var golds = new List<int>();
            _player.OnPlayerGoldChange += golds.Add;

            _player.GivePlayerGold(2);
            _player.GivePlayerGold(3);

            Assert.AreEqual(5, _player.Gold);
            CollectionAssert.AreEqual(new[] { 2, 5 }, golds);
        }

        [Test]
        public void NegativeScoreOrGold_IsIgnored()
        {
            _player.GivePlayerScore(2);
            _player.GivePlayerGold(2);

            _player.GivePlayerScore(-1);
            _player.GivePlayerGold(-1);

            Assert.AreEqual(2, _player.Score);
            Assert.AreEqual(2, _player.Gold);
        }

        [Test]
        public void ResetPlayer_StartsANewRun_AndKeepsHighScoreAndGold()
        {
            var moves = new List<GridPosition>();
            _player.OnPlayerMove += (from, to) => moves.Add(to);
            _player.MovePlayer(GridDirection.Up);
            _player.GivePlayerScore(4);
            _player.GivePlayerGold(2);
            _player.DamagePlayer(1);

            _player.ResetPlayer();

            Assert.IsTrue(_player.IsPlayerAlive());
            Assert.AreEqual(_start, _player.GetPlayerGridPosition());
            Assert.AreEqual(_start, moves[moves.Count - 1]);
            Assert.AreEqual(0, _player.Score);
            Assert.AreEqual(4, _player.HighScore);
            Assert.AreEqual(2, _player.Gold);
        }
    }
}
