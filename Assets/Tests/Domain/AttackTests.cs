using Domain.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Tests
{
    [TestFixture]
    public class AttackTests
    {
        private IArena _arena;

        [SetUp]
        public void SetUp()
        {
            _arena = new RectangleArena(3, 4, new RandomSource(0));
        }

        // Each stage lasts one second, so one tick of one second moves to the next stage.
        private HammerAttack OneSecondStages(GridPosition middle = null)
        {
            return new HammerAttack(middle ?? new GridPosition(1, 1), HorizontalDirection.LeftToRight,
                new AttackTiming(1f, 1f, 1f, 1f), _arena);
        }

        [Test]
        public void NewAttack_HasNotStarted()
        {
            var attack = OneSecondStages();

            Assert.AreEqual(AttackStage.None, attack.CurrentStage);
            Assert.IsFalse(attack.IsComplete());
            Assert.IsFalse(attack.HasDealtDamage);
        }

        [Test]
        public void Tick_BeforeStart_DoesNothing()
        {
            var attack = OneSecondStages();

            attack.OnTick(5f);

            Assert.AreEqual(AttackStage.None, attack.CurrentStage);
        }

        [Test]
        public void Start_EntersTelegraph_AndReportsIt()
        {
            var attack = OneSecondStages();
            var stages = new List<AttackStage>();
            attack.OnStageChanged += (a, stage) => stages.Add(stage);

            attack.Start();

            Assert.AreEqual(AttackStage.Telegraph, attack.CurrentStage);
            CollectionAssert.AreEqual(new[] { AttackStage.Telegraph }, stages);
        }

        [Test]
        public void Start_Twice_DoesNotRestart()
        {
            var attack = OneSecondStages();
            attack.Start();
            attack.OnTick(1f);

            attack.Start();

            Assert.AreEqual(AttackStage.Commit, attack.CurrentStage);
        }

        [Test]
        public void Ticking_MovesThroughEveryStageInOrder()
        {
            var attack = OneSecondStages();
            var stages = new List<AttackStage>();
            attack.OnStageChanged += (a, stage) => stages.Add(stage);
            int completions = 0;
            attack.OnAttackComplete += () => completions++;

            attack.Start();
            for (int i = 0; i < 4; i++) attack.OnTick(1f);

            CollectionAssert.AreEqual(
                new[] { AttackStage.Telegraph, AttackStage.Commit, AttackStage.Impact, AttackStage.Recovery }, stages);
            Assert.IsTrue(attack.IsComplete());
            Assert.AreEqual(1, completions);
        }

        [Test]
        public void Tick_AfterComplete_DoesNothing()
        {
            var attack = OneSecondStages();
            int completions = 0;
            attack.OnAttackComplete += () => completions++;
            attack.Start();
            for (int i = 0; i < 4; i++) attack.OnTick(1f);

            attack.OnTick(1f);

            Assert.AreEqual(AttackStage.Complete, attack.CurrentStage);
            Assert.AreEqual(1, completions);
        }

        [Test]
        public void StageProgress_IsTheShareOfTheStageElapsed()
        {
            var attack = OneSecondStages();
            attack.Start();

            attack.OnTick(0.25f);

            Assert.AreEqual(0.25f, attack.GetStageProgress(), 1e-5f);
            Assert.AreEqual(AttackStage.Telegraph, attack.GetCurrentState().Stage);
            Assert.AreEqual(0.25f, attack.GetCurrentState().ProgressInStage, 1e-5f);
        }

        [Test]
        public void StageProgress_OfAZeroLengthStage_IsFull()
        {
            var attack = new HammerAttack(new GridPosition(1, 1), HorizontalDirection.LeftToRight,
                new AttackTiming(0f, 1f, 1f, 1f), _arena);
            attack.Start();

            Assert.AreEqual(1f, attack.GetStageProgress());
        }

        [Test]
        public void HammerEndangersItsTiles_OnlyDuringImpact()
        {
            var attack = OneSecondStages();
            var middle = new GridPosition(1, 1);
            attack.Start();

            Assert.IsFalse(attack.IsPositionUnderAttack(middle), "telegraph");
            attack.OnTick(1f);
            Assert.IsFalse(attack.IsPositionUnderAttack(middle), "commit");
            attack.OnTick(1f);
            Assert.IsTrue(attack.IsPositionUnderAttack(middle), "impact");
            attack.OnTick(1f);
            Assert.IsFalse(attack.IsPositionUnderAttack(middle), "recovery");
        }

        [Test]
        public void MarkDamageDealt_IsRemembered()
        {
            var attack = OneSecondStages();

            attack.MarkDamageDealt();

            Assert.IsTrue(attack.HasDealtDamage);
        }

        [Test]
        public void Hammer_LeftToRight_HitsMiddleAboveBelowAndRight()
        {
            var attack = OneSecondStages(new GridPosition(1, 1));

            CollectionAssert.AreEquivalent(
                new[] { new GridPosition(1, 1), new GridPosition(1, 2), new GridPosition(1, 0), new GridPosition(2, 1) },
                attack.Pattern.GetAffectedArea().ToList());
        }

        [Test]
        public void Hammer_RightToLeft_HitsMiddleAboveBelowAndLeft()
        {
            var attack = new HammerAttack(new GridPosition(1, 1), HorizontalDirection.RightToLeft,
                new AttackTiming(1f, 1f, 1f, 1f), _arena);

            CollectionAssert.AreEquivalent(
                new[] { new GridPosition(1, 1), new GridPosition(1, 2), new GridPosition(1, 0), new GridPosition(0, 1) },
                attack.Pattern.GetAffectedArea().ToList());
        }

        [Test]
        public void Hammer_InACorner_LeavesOutTilesOffTheArena()
        {
            var attack = OneSecondStages(new GridPosition(2, 0));

            CollectionAssert.AreEquivalent(
                new[] { new GridPosition(2, 0), new GridPosition(2, 1) },
                attack.Pattern.GetAffectedArea().ToList());
        }

        [Test]
        public void SpearRow_CoversItsWholeRow()
        {
            var spear = new SpearRowAttack(2, HorizontalDirection.LeftToRight, new AttackTiming(1f, 1f, 1f, 1f), _arena);

            CollectionAssert.AreEquivalent(
                new[] { new GridPosition(0, 2), new GridPosition(1, 2), new GridPosition(2, 2) },
                spear.Pattern.GetAffectedArea().ToList());
        }

        [Test]
        public void SpearColumn_CoversItsWholeColumn()
        {
            var spear = new SpearColumnAttack(0, VerticalDirection.BottomToTop, new AttackTiming(1f, 1f, 1f, 1f), _arena);

            CollectionAssert.AreEquivalent(
                new[] { new GridPosition(0, 0), new GridPosition(0, 1), new GridPosition(0, 2), new GridPosition(0, 3) },
                spear.Pattern.GetAffectedArea().ToList());
        }
    }
}
