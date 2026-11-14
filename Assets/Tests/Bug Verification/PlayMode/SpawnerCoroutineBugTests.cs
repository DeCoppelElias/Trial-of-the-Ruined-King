using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Domain.Services;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace BugVerification
{
    [TestFixture]
    public class SpawnerCoroutineBugTests
    {
        private GameObject _go;
        private GameStateService _state;
        private TimeService _time;
        private AttackService _attacks;
        private int _spawned;

        private void CreateAttacker(int attacksPerWave)
        {
            // Tall arena so the row spear factory never runs out of free rows.
            var arena = new RectangleArena(3, 10, Vector3.zero);
            _state = new GameStateService();
            _time = new TimeService(_state);
            _attacks = new AttackService(_time, _state);
            _spawned = 0;
            _attacks.OnAttackAdded += _ => _spawned++;

            _go = new GameObject("PlayerAttacker under test");
            var attacker = _go.AddComponent<PlayerAttacker>();
            var definition = ScriptableObject.CreateInstance<PlayTestSpearRowDefinition>();
            definition.attackTiming = new AttackTiming(5f, 0.4f, 0f, 0f);

            Set(attacker, "_attackDefinitions", new List<AttackDefinitionBase> { definition });
            Set(attacker, "_attackSequences", new List<AttackSequenceDefinition>());
            Set(attacker, "_minSimultaneousAttacks", attacksPerWave);
            Set(attacker, "_maxSimultaneousAttacks", attacksPerWave);
            Set(attacker, "_minAttackOffset", 0.2f);
            Set(attacker, "_maxAttackOffset", 0.2f);
            Set(attacker, "_initialDelay", 0f);
            Set(attacker, "_timeBetweenAttacks", 0.01f);

            attacker.Initialize(_attacks, arena, _time, _state);
            _state.TransitionTo(GameState.Gameplay);
        }

        private static void Set(object target, string field, object value)
        {
            target.GetType().GetField(field, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, value);
        }

        [TearDown]
        public void TearDown()
        {
            if (_go != null) Object.Destroy(_go);
        }

        [UnityTest, Category("KnownBug")]
        public IEnumerator B3_ReturningToMenu_CancelsTheWaveInProgress()
        {
            CreateAttacker(4);
            _time.Tick(0.02f);
            Assume.That(_spawned, Is.EqualTo(1));

            _state.TransitionTo(GameState.MainMenu);
            yield return new WaitForSeconds(1f);

            Assert.AreEqual(1, _spawned, "the wave kept spawning after StopSpawning");
            Assert.IsEmpty(_attacks.GetActiveAttacks(), "attacks are waiting in the main menu");
        }

        [UnityTest, Category("KnownBug")]
        public IEnumerator B3_NewWave_ReplacesThePreviousWave()
        {
            CreateAttacker(3);
            _time.Tick(0.02f); // wave 1 starts
            _time.Tick(0.02f); // wave 2 starts and is meant to stop wave 1
            yield return new WaitForSeconds(1f);

            // Wave 1 spawned 1 before it was replaced, wave 2 spawns 3.
            Assert.AreEqual(4, _spawned, "the replaced wave kept spawning alongside the new one");
        }

        [UnityTest, Category("KnownBug")]
        public IEnumerator B5_WaveWaits_WhileDomainTimeIsPaused()
        {
            CreateAttacker(3);
            _time.Tick(0.02f);
            Assume.That(_spawned, Is.EqualTo(1));

            // PlayerHitPresenter pauses domain time on every hit; death slows it to zero.
            _time.Pause();
            yield return new WaitForSeconds(1f);

            Assert.AreEqual(1, _spawned, "attacks spawned while game time was frozen");
        }
    }
}
