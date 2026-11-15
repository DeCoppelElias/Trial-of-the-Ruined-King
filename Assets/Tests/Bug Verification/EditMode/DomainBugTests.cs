using Domain.Ports.Driven;
using System.Collections.Generic;
using Domain.Models.Player;
using Domain.Services;
using NUnit.Framework;
using UnityEngine;

namespace BugVerification
{
    // Reproductions for docs/attack-system-review.md.
    // Each test asserts the intended behaviour, so a failing test means the bug is present.
    // Category "KnownBug" marks tests expected to fail until the bug is fixed.
    [TestFixture]
    public class DomainBugTests
    {
        private RectangleArena _arena;
        private GameStateService _state;
        private TimeService _time;
        private AttackService _attacks;
        private PlayerService _player;
        private readonly GridPosition _playerStart = new GridPosition(1, 1);

        [SetUp]
        public void SetUp()
        {
            // Same wiring order as GameBootstrap.Awake, same arena size as the scene.
            _arena = new RectangleArena(3, 4, new System.Random(0));
            _state = new GameStateService();
            _time = new TimeService(_state);
            _attacks = new AttackService(_time, _state);
            _player = new PlayerService(new Player(1, _playerStart), _arena, _attacks, _time);
            _state.TransitionTo(GameState.Gameplay);
        }

        private HammerAttack HammerOnPlayer(float telegraph)
        {
            return new HammerAttack(_playerStart, HorizontalDirection.LeftToRight,
                new AttackTiming(telegraph, 0.05f, 0.1f, 0.1f), _arena);
        }

        private void Run(float seconds, float dt = 0.02f)
        {
            int ticks = Mathf.CeilToInt(seconds / dt);
            for (int i = 0; i < ticks; i++) _time.Tick(dt);
        }

        [Test, Category("KnownBug")]
        public void B1_PlayerDiesOnlyOnce_WhenHitAgainAfterDeath()
        {
            int deaths = 0;
            _player.OnPlayerDied += () => deaths++;

            _attacks.SpawnAttack(HammerOnPlayer(0.1f));
            _attacks.SpawnAttack(HammerOnPlayer(0.4f));
            Run(1f);

            Assert.AreEqual(1, deaths, "OnPlayerDied fired once per hit, not once per death");
        }

        [Test, Category("KnownBug")]
        public void B2_ScoreFreezes_AfterDeath()
        {
            int scoreAtDeath = -1;
            _player.OnPlayerDied += () => scoreAtDeath = _player.Score;

            _attacks.SpawnAttack(HammerOnPlayer(0.1f));
            _attacks.SpawnAttack(new SpearRowAttack(3, HorizontalDirection.LeftToRight,
                new AttackTiming(0.5f, 0.4f, 0f, 0f), _arena));
            Run(2f);

            Assume.That(scoreAtDeath, Is.EqualTo(0));
            Assert.AreEqual(scoreAtDeath, _player.Score, "attacks completing after death still award score");
        }

        [Test, Category("KnownBug")]
        public void B2_ReturningToMenu_DoesNotAwardOrSaveScoreForCancelledAttacks()
        {
            // Subscribed after AttackService, exactly like GameBootstrap.InitializePlayerComponents.
            var storage = new MemoryStorage();
            var persistence = new PlayerStatsPersistence(_player, storage);
            _state.OnStateChanged += (prev, next) =>
            {
                if (prev == GameState.Gameplay && next != GameState.Gameplay) persistence.HandleExitGameplay();
            };

            for (int row = 0; row < 3; row++)
            {
                _attacks.SpawnAttack(new SpearRowAttack(row, HorizontalDirection.LeftToRight,
                    new AttackTiming(10f, 0.4f, 0f, 0f), _arena));
            }
            Run(0.1f);
            _state.TransitionTo(GameState.MainMenu);

            Assert.AreEqual(0, _player.Score, "cancelled attacks awarded score");
            Assert.AreEqual(0, storage.Load<int>(StorageKeys.Highscore.ToString()), "inflated high score was saved");
        }

        // Spear Column definition: commit 0.4s, scaled by the scene's minimum timing multiplier 0.5.
        [TestCase(1f / 60f)]
        [TestCase(1f / 30f)]
        [TestCase(1f / 12f, Category = "KnownBug")]
        public void B7_SpearColumn_AtMaxDifficulty_EndangersEveryTile(float dt)
        {
            var spear = new SpearColumnAttack(0, VerticalDirection.BottomToTop,
                new AttackTiming(0.5f, 0.2f, 0f, 0f), _arena);
            _attacks.SpawnAttack(spear);

            var visitedRows = new HashSet<int>();
            for (int i = 0; i < 500 && !spear.IsComplete(); i++)
            {
                _time.Tick(dt);
                foreach (var pos in spear.Pattern.GetActiveDangerPositions(spear.GetCurrentState()))
                    visitedRows.Add(pos.GridY);
            }

            CollectionAssert.AreEquivalent(new[] { 0, 1, 2, 3 }, visitedRows, "a tile was skipped and could never hit the player");
        }

        [TestCase(1f / 60f, Category = "KnownBug")]
        [TestCase(1f / 30f, Category = "KnownBug")]
        public void N1_AttackLasts_ItsConfiguredDuration(float dt)
        {
            var timing = new AttackTiming(1f, 0.4f, 0f, 0f); // Spear Row definition
            var spear = new SpearRowAttack(3, HorizontalDirection.LeftToRight, timing, _arena);
            _attacks.SpawnAttack(spear);

            float elapsed = 0f;
            while (!spear.IsComplete() && elapsed < 10f)
            {
                _time.Tick(dt);
                elapsed += dt;
            }

            Assert.That(elapsed, Is.EqualTo(timing.GetTotalDuration()).Within(dt),
                "every stage change drops the leftover time of that tick");
        }

        private class MemoryStorage : IStorage
        {
            private readonly Dictionary<string, object> _data = new Dictionary<string, object>();
            public void Save<T>(string key, T data) => _data[key] = data;
            public T Load<T>(string key) => _data.TryGetValue(key, out var value) ? (T)value : default;
            public bool HasKey(string key) => _data.ContainsKey(key);
            public void Delete(string key) => _data.Remove(key);
        }
    }
}
