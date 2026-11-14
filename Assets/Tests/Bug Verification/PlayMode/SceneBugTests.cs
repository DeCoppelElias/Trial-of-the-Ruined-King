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
    [TestFixture]
    public class SceneBugTests
    {
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

        [UnityTest]
        public IEnumerator B4_DangerTiles_AreCleanAfterQuittingMidWarning()
        {
            yield return SceneManager.LoadSceneAsync("SampleScene");
            yield return null;

            var boot = Object.FindFirstObjectByType<GameBootstrap>();
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

            var counts = Get<Dictionary<GridPosition, int>>(tileVisualizer, "_dangerReferenceCounts");
            Assume.That(counts[new GridPosition(0, 3)], Is.EqualTo(1), "warning was never shown");

            state.TransitionTo(GameState.MainMenu);
            yield return null;
            yield return StartRunAndWaitForArena(state, attacker, arenaPresenter);

            var tiles = Get<Dictionary<GridPosition, Tile>>(tileVisualizer, "_tiles");
            foreach (var pair in tiles)
            {
                Assert.AreEqual(0, counts[pair.Key], $"reference count leaked at {pair.Key}");
                Assert.IsFalse(pair.Value.IsDangerIndicatorActive(), $"danger indicator stuck on at {pair.Key}");
            }
        }
    }
}
