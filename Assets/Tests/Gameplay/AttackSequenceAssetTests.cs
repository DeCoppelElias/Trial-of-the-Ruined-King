using Domain.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Gameplay.Tests
{
    // Checks the attack sequence assets themselves. Step parameters are [SerializeReference],
    // so renaming or moving a parameter type empties them silently, and the game only
    // notices when that sequence is picked. See docs/adr/ports-and-adapters.md.
    [TestFixture]
    public class AttackSequenceAssetTests
    {
        // The arena size GameBootstrap uses in the scene.
        private const int ArenaWidth = 3;
        private const int ArenaHeight = 4;

        private static IEnumerable<TestCaseData> AllSequences()
        {
            return AssetDatabase.FindAssets("t:" + nameof(AttackSequenceDefinition))
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => new TestCaseData(path).SetName(System.IO.Path.GetFileNameWithoutExtension(path)));
        }

        [Test]
        public void SequenceAssetsExist()
        {
            Assert.IsNotEmpty(AllSequences().ToList(), "no attack sequence assets were found");
        }

        [TestCaseSource(nameof(AllSequences))]
        public void Sequence_IsValid(string path)
        {
            var sequence = AssetDatabase.LoadAssetAtPath<AttackSequenceDefinition>(path);
            var arena = new RectangleArena(ArenaWidth, ArenaHeight, new RandomSource(0));

            Assert.IsTrue(sequence.Validate(arena, out string error), error);
        }
    }
}
