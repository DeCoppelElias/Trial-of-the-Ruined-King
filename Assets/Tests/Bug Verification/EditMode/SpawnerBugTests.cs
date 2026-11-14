using System.Collections.Generic;
using System.Reflection;
using Domain.Services;
using NUnit.Framework;
using UnityEngine;

namespace BugVerification
{
    [TestFixture]
    public class SpawnerBugTests
    {
        [Test, Category("DesignQuestion")]
        public void B6_RandomSpears_LeaveSomeOfTheArenaFree()
        {
            var arena = new RectangleArena(3, 4, Vector3.zero);
            var definition = ScriptableObject.CreateInstance<TestSpearRowDefinition>();
            definition.attackTiming = new AttackTiming(1f, 0.4f, 0f, 0f);
            var factory = new SpearRowAttackFactory(arena);
            var occupied = new HashSet<GridPosition>();

            try
            {
                // The scene allows up to 4 simultaneous attacks with _minFreePercentage 0.5.
                for (int i = 0; i < 4; i++)
                {
                    var attack = factory.CreateRandomAttack(definition, 1f, occupied, 0.5f);
                    if (attack == null) continue;
                    foreach (var pos in attack.Pattern.GetAffectedArea()) occupied.Add(pos);
                }
            }
            finally
            {
                Object.DestroyImmediate(definition);
            }

            Assert.Less(occupied.Count, 12, "four row spears covered all 12 tiles");
        }

        [Test, Category("KnownBug")]
        public void B6_TileStaysTracked_WhileAnotherAttackStillCoversIt()
        {
            var arena = new RectangleArena(3, 4, Vector3.zero);
            var state = new GameStateService();
            var time = new TimeService(state);
            var attacks = new AttackService(time, state);
            state.TransitionTo(GameState.Gameplay);

            var go = new GameObject("PlayerAttacker under test");
            bool stillTracked;
            try
            {
                var attacker = go.AddComponent<PlayerAttacker>();
                attacker.Initialize(attacks, arena, time, state);

                // Spear on row 0 finishes quickly. Hammer at (0,1) also covers (0,0) and stays active.
                attacks.SpawnAttack(new SpearRowAttack(0, HorizontalDirection.LeftToRight,
                    new AttackTiming(0.05f, 0.1f, 0f, 0f), arena));
                attacks.SpawnAttack(new HammerAttack(new GridPosition(0, 1), HorizontalDirection.LeftToRight,
                    new AttackTiming(10f, 0.2f, 0.1f, 1f), arena));
                for (int i = 0; i < 50; i++) time.Tick(0.02f);

                var tracked = (HashSet<GridPosition>)typeof(PlayerAttacker)
                    .GetField("_attackedTiles", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(attacker);
                stillTracked = tracked.Contains(new GridPosition(0, 0));
            }
            finally
            {
                Object.DestroyImmediate(go);
            }

            Assert.IsTrue(stillTracked, "tile (0,0) dropped from tracking while the hammer still covers it");
        }
    }
}
