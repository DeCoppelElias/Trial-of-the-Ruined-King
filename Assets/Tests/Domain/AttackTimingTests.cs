using Domain.Models;
using NUnit.Framework;

namespace Domain.Tests
{
    [TestFixture]
    public class AttackTimingTests
    {
        private readonly AttackTiming _timing = new AttackTiming(1f, 2f, 3f, 4f);

        [Test]
        public void GetTotalDuration_SumsEveryStage()
        {
            Assert.AreEqual(10f, _timing.GetTotalDuration());
        }

        [TestCase(AttackStage.Telegraph, 1f)]
        [TestCase(AttackStage.Commit, 2f)]
        [TestCase(AttackStage.Impact, 3f)]
        [TestCase(AttackStage.Recovery, 4f)]
        [TestCase(AttackStage.None, 0f)]
        [TestCase(AttackStage.Complete, 0f)]
        public void GetDurationForStage_ReturnsThatStagesDuration(AttackStage stage, float expected)
        {
            Assert.AreEqual(expected, _timing.GetDurationForStage(stage));
        }

        [Test]
        public void Multiply_ScalesEveryStage_AndLeavesTheOriginalAlone()
        {
            var scaled = _timing.Multiply(0.5f);

            Assert.AreEqual(0.5f, scaled.TelegraphDuration);
            Assert.AreEqual(1f, scaled.CommitDuration);
            Assert.AreEqual(1.5f, scaled.ImpactDuration);
            Assert.AreEqual(2f, scaled.RecoveryDuration);
            Assert.AreEqual(10f, _timing.GetTotalDuration());
        }
    }
}
