using Domain.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Tests
{
    // The factories make gameplay rolls, so the same seed must build the same
    // attacks. See docs/adr/randomness.md.
    [TestFixture]
    public class AttackFactoryTests
    {
        private class TestDefinition : AttackDefinitionBase
        {
            public override Type AttackType => typeof(SpearRowAttack);
        }

        private static readonly HashSet<GridPosition> NoTiles = new HashSet<GridPosition>();

        private static string Describe(Attack attack)
        {
            return attack switch
            {
                SpearRowAttack row => $"row {row.TargetRow} {row.Direction}",
                SpearColumnAttack column => $"column {column.TargetColumn} {column.Direction}",
                HammerAttack hammer => $"hammer {hammer.Middle} {hammer.Direction}",
                null => "none",
                _ => attack.GetType().Name
            };
        }

        private static List<string> Build(Func<IArena, RandomSource, IAttackFactory> make, int seed)
        {
            var arena = new RectangleArena(3, 4, new RandomSource(0));
            var factory = make(arena, new RandomSource(seed));
            var definition = ScriptableObject.CreateInstance<TestDefinition>();
            definition.attackTiming = new AttackTiming(1, 1, 1, 1);

            var built = new List<string>();
            for (int i = 0; i < 20; i++)
                built.Add(Describe(factory.CreateRandomAttack(definition, 1f, NoTiles, 0f)));

            UnityEngine.Object.DestroyImmediate(definition);
            return built;
        }

        private static IEnumerable<TestCaseData> Factories()
        {
            yield return new TestCaseData((Func<IArena, RandomSource, IAttackFactory>)((a, r) => new SpearRowAttackFactory(a, r))).SetName("SpearRow_SameSeed_BuildsTheSameAttacks");
            yield return new TestCaseData((Func<IArena, RandomSource, IAttackFactory>)((a, r) => new SpearColumnAttackFactory(a, r))).SetName("SpearColumn_SameSeed_BuildsTheSameAttacks");
            yield return new TestCaseData((Func<IArena, RandomSource, IAttackFactory>)((a, r) => new HammerAttackFactory(a, r))).SetName("Hammer_SameSeed_BuildsTheSameAttacks");
        }

        [TestCaseSource(nameof(Factories))]
        public void SameSeed_BuildsTheSameAttacks(Func<IArena, RandomSource, IAttackFactory> make)
        {
            var first = Build(make, 9);

            CollectionAssert.DoesNotContain(first, "none", "every roll should build an attack on an empty arena");
            CollectionAssert.AreEqual(first, Build(make, 9));
        }
    }
}
