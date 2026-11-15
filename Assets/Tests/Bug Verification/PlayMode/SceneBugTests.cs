using Domain.Models;
using Domain.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace BugVerification
{
    // Runs against the real SampleScene and its GameBootstrap wiring.
    // The real bootstrap saves through PlayerPrefs, so the saved stats are restored afterwards
    // and the scene is unloaded so it cannot leak into later tests.
    [TestFixture]
    public class SceneBugTests
    {
        private const string SceneName = "SampleScene";
        private readonly Dictionary<string, int?> _savedPrefs = new Dictionary<string, int?>();

        [SetUp]
        public void SavePlayerPrefs()
        {
            _savedPrefs.Clear();
            foreach (StorageKeys key in Enum.GetValues(typeof(StorageKeys)))
            {
                string name = key.ToString();
                _savedPrefs[name] = PlayerPrefs.HasKey(name) ? PlayerPrefs.GetInt(name) : (int?)null;
            }
        }

        [UnityTearDown]
        public IEnumerator RestorePlayerPrefsAndUnloadScene()
        {
            Scene scene = SceneManager.GetSceneByName(SceneName);
            if (scene.isLoaded)
            {
                SceneManager.SetActiveScene(SceneManager.CreateScene("Empty test scene"));
                yield return SceneManager.UnloadSceneAsync(scene);
            }

            foreach (var pair in _savedPrefs)
            {
                if (pair.Value.HasValue) PlayerPrefs.SetInt(pair.Key, pair.Value.Value);
                else PlayerPrefs.DeleteKey(pair.Key);
            }
            PlayerPrefs.Save();
        }

        private static T Get<T>(object target, string field)
        {
            return (T)target.GetType().GetField(field, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(target);
        }

        private static IEnumerator StartRunAndWaitForArena(GameStateService state, PlayerAttacker attacker, ArenaPresenter arenaPresenter)
        {
            bool built = false;
            void OnBuilt() => built = true;
            arenaPresenter.OnArenaVisualized += OnBuilt;

            state.TransitionTo(GameState.Gameplay);
            attacker.StopSpawning(); // keep random waves out of the test

            float timeout = Time.realtimeSinceStartup + 15f;
            while (!built && Time.realtimeSinceStartup < timeout) yield return null;
            arenaPresenter.OnArenaVisualized -= OnBuilt;
            Assert.IsTrue(built, "arena never finished building");
        }

        // B4: a presentation destroyed mid-warning never releases its warning.
        // Rebuilding the arena hides this by resetting every count, so the check runs before the rebuild.
        [UnityTest, Category("KnownBug")]
        public IEnumerator B4_QuittingMidWarning_ReleasesTheWarning()
        {
            yield return SceneManager.LoadSceneAsync(SceneName);
            yield return null;

            var boot = UnityEngine.Object.FindFirstObjectByType<GameBootstrap>();
            var state = Get<GameStateService>(boot, "_gameStateService");
            var attacks = Get<AttackService>(boot, "_attackService");
            var arena = Get<RectangleArena>(boot, "_arena");
            var attacker = Get<PlayerAttacker>(boot, "_playerAttacker");
            var arenaPresenter = Get<ArenaPresenter>(boot, "_arenaPresenter");
            var tileVisualizer = Get<TileVisualizer>(boot, "_tileVisualizer");

            yield return StartRunAndWaitForArena(state, attacker, arenaPresenter);

            // Row 3 is away from the player's start tile (1,1). Long commit keeps the warning on.
            attacks.SpawnAttack(new SpearRowAttack(3, HorizontalDirection.LeftToRight,
                new AttackTiming(0.2f, 10f, 0f, 0f), arena));
            yield return new WaitForSeconds(0.5f);

            var warned = new GridPosition(0, 3);
            Assume.That(tileVisualizer.GetDangerReferenceCount(warned), Is.EqualTo(1), "warning was never shown");

            state.TransitionTo(GameState.MainMenu);
            yield return null;

            Assert.AreEqual(0, tileVisualizer.GetDangerReferenceCount(warned),
                "the cancelled spear still holds its warning on (0,3)");
        }
    }
}
