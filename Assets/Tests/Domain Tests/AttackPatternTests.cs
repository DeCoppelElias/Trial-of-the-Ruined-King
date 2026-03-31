using NUnit.Framework;
using System.Linq;

namespace Tests
{
    [TestFixture]
    public class AttackPatternTests
    {
        private IArena _arena;

        [SetUp]
        public void SetUp()
        {
            _arena = new RectangleArena(5, 5, UnityEngine.Vector3.zero);
        }

        [Test]
        public void InstantPattern_ReturnsAllPositions_DuringImpactStage()
        {
            var pattern = new InstantAreaPattern(new GridPosition(2, 2), _arena);
            var state = new AttackState(AttackStage.Impact, 0.5f);

            var dangerous = pattern.GetActiveDangerPositions(state).ToList();

            Assert.AreEqual(1, dangerous.Count);
            Assert.AreEqual(new GridPosition(2, 2), dangerous[0]);
        }

        [Test]
        public void InstantPattern_ReturnsEmpty_DuringNonImpactStages()
        {
            var pattern = new InstantAreaPattern(new GridPosition(2, 2), _arena);

            Assert.IsEmpty(pattern.GetActiveDangerPositions(new AttackState(AttackStage.Telegraph, 0.5f)));
            Assert.IsEmpty(pattern.GetActiveDangerPositions(new AttackState(AttackStage.Commit, 0.5f)));
            Assert.IsEmpty(pattern.GetActiveDangerPositions(new AttackState(AttackStage.Recovery, 0.5f)));
        }

        [Test]
        public void InstantPattern_GetAffectedArea_ReturnsAllPositions()
        {
            var pattern = new InstantAreaPattern(new GridPosition(2, 2), _arena);

            var affected = pattern.GetAffectedArea().ToList();

            Assert.AreEqual(1, affected.Count);
            Assert.AreEqual(new GridPosition(2, 2), affected[0]);
        }

        [Test]
        public void ProgressivePattern_ReturnsFirstPosition_AtCommitStart()
        {
            var pattern = new LinearProgressivePattern(0, HorizontalDirection.LeftToRight, _arena);
            var state = new AttackState(AttackStage.Commit, 0f);

            var dangerous = pattern.GetActiveDangerPositions(state).ToList();

            Assert.AreEqual(1, dangerous.Count);
            Assert.AreEqual(new GridPosition(0, 0), dangerous[0]);
        }

        [Test]
        public void ProgressivePattern_ReturnsLastPosition_AtCommitEnd()
        {
            var pattern = new LinearProgressivePattern(0, HorizontalDirection.LeftToRight, _arena);
            var state = new AttackState(AttackStage.Commit, 1f);

            var dangerous = pattern.GetActiveDangerPositions(state).ToList();

            Assert.AreEqual(1, dangerous.Count);
            Assert.AreEqual(new GridPosition(4, 0), dangerous[0]);
        }

        [Test]
        public void ProgressivePattern_ProgressesThroughTiles()
        {
            var pattern = new LinearProgressivePattern(0, HorizontalDirection.LeftToRight, _arena);

            var pos1 = pattern.GetActiveDangerPositions(new AttackState(AttackStage.Commit, 0.25f)).First();
            var pos2 = pattern.GetActiveDangerPositions(new AttackState(AttackStage.Commit, 0.5f)).First();
            var pos3 = pattern.GetActiveDangerPositions(new AttackState(AttackStage.Commit, 0.75f)).First();
            var pos4 = pattern.GetActiveDangerPositions(new AttackState(AttackStage.Commit, 1f)).First();

            Assert.AreEqual(new GridPosition(1, 0), pos1);
            Assert.AreEqual(new GridPosition(2, 0), pos2);
            Assert.AreEqual(new GridPosition(3, 0), pos3);
            Assert.AreEqual(new GridPosition(4, 0), pos4);
        }

        [Test]
        public void ProgressivePattern_ReturnsEmpty_DuringRecovery()
        {
            var pattern = new LinearProgressivePattern(0, HorizontalDirection.LeftToRight, _arena);
            var state = new AttackState(AttackStage.Recovery, 0.5f);

            var dangerous = pattern.GetActiveDangerPositions(state).ToList();

            Assert.IsEmpty(dangerous);
        }

        [Test]
        public void ProgressivePattern_HoldsAtFinalPosition_DuringImpact()
        {
            var pattern = new LinearProgressivePattern(0, HorizontalDirection.LeftToRight, _arena);

            var posAtStart = pattern.GetActiveDangerPositions(new AttackState(AttackStage.Impact, 0f)).First();
            var posAtMid = pattern.GetActiveDangerPositions(new AttackState(AttackStage.Impact, 0.5f)).First();
            var posAtEnd = pattern.GetActiveDangerPositions(new AttackState(AttackStage.Impact, 1f)).First();

            Assert.AreEqual(new GridPosition(4, 0), posAtStart);
            Assert.AreEqual(new GridPosition(4, 0), posAtMid);
            Assert.AreEqual(new GridPosition(4, 0), posAtEnd);
        }

        [Test]
        public void ProgressivePattern_RightToLeft_ReversesDirection()
        {
            var pattern = new LinearProgressivePattern(0, HorizontalDirection.RightToLeft, _arena);

            var firstPos = pattern.GetActiveDangerPositions(new AttackState(AttackStage.Commit, 0f)).First();
            var lastPos = pattern.GetActiveDangerPositions(new AttackState(AttackStage.Commit, 1f)).First();

            Assert.AreEqual(new GridPosition(4, 0), firstPos);
            Assert.AreEqual(new GridPosition(0, 0), lastPos);
        }

        [Test]
        public void ProgressivePattern_Vertical_BottomToTop()
        {
            var pattern = new LinearProgressivePattern(0, VerticalDirection.BottomToTop, _arena);

            var firstPos = pattern.GetActiveDangerPositions(new AttackState(AttackStage.Commit, 0f)).First();
            var lastPos = pattern.GetActiveDangerPositions(new AttackState(AttackStage.Commit, 1f)).First();

            Assert.AreEqual(new GridPosition(0, 0), firstPos);
            Assert.AreEqual(new GridPosition(0, 4), lastPos);
        }

        [Test]
        public void ProgressivePattern_Vertical_TopToBottom()
        {
            var pattern = new LinearProgressivePattern(0, VerticalDirection.TopToBottom, _arena);

            var firstPos = pattern.GetActiveDangerPositions(new AttackState(AttackStage.Commit, 0f)).First();
            var lastPos = pattern.GetActiveDangerPositions(new AttackState(AttackStage.Commit, 1f)).First();

            Assert.AreEqual(new GridPosition(0, 4), firstPos);
            Assert.AreEqual(new GridPosition(0, 0), lastPos);
        }

        [Test]
        public void ProgressivePattern_GetAffectedArea_ReturnsEntireRow()
        {
            var pattern = new LinearProgressivePattern(0, HorizontalDirection.LeftToRight, _arena);

            var affected = pattern.GetAffectedArea().ToList();

            Assert.AreEqual(5, affected.Count);
            for (int x = 0; x < 5; x++)
            {
                Assert.IsTrue(affected.Contains(new GridPosition(x, 0)));
            }
        }
    }
}