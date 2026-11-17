using Domain.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Domain.Tests
{
    [TestFixture]
    public class RandomSourceTests
    {
        private static List<float> Sequence(RandomSource random)
        {
            var values = new List<float>();
            for (int i = 0; i < 20; i++)
            {
                values.Add(random.Range(0, 100));
                values.Add(random.Range(-1f, 1f));
                values.Add(random.Value());
                values.Add(random.CoinFlip() ? 1 : 0);
                values.Add(random.Pick(new[] { 10, 20, 30 }));
            }
            return values;
        }

        [Test]
        public void SameSeed_GivesTheSameRolls()
        {
            CollectionAssert.AreEqual(Sequence(new RandomSource(7)), Sequence(new RandomSource(7)));
        }

        [Test]
        public void DifferentSeeds_GiveDifferentRolls()
        {
            CollectionAssert.AreNotEqual(Sequence(new RandomSource(7)), Sequence(new RandomSource(8)));
        }

        [Test]
        public void Reseed_RestartsTheSequence()
        {
            var random = new RandomSource(3);
            var first = Sequence(random);

            random.Reseed(3);

            CollectionAssert.AreEqual(first, Sequence(random));
            Assert.AreEqual(3, random.Seed);
        }

        [Test]
        public void IntRange_StaysWithinMinInclusiveMaxExclusive()
        {
            var random = new RandomSource(0);
            for (int i = 0; i < 1000; i++)
                Assert.That(random.Range(2, 5), Is.InRange(2, 4));
        }

        [Test]
        public void FloatRange_StaysWithinBounds()
        {
            var random = new RandomSource(0);
            for (int i = 0; i < 1000; i++)
                Assert.That(random.Range(0.8f, 1.2f), Is.InRange(0.8f, 1.2f));
        }

        [Test]
        public void Value_StaysBetweenZeroAndOne()
        {
            var random = new RandomSource(0);
            for (int i = 0; i < 1000; i++)
                Assert.That(random.Value(), Is.InRange(0f, 1f));
        }

        [Test]
        public void Pick_FromEmptyList_Throws()
        {
            Assert.Throws<ArgumentException>(() => new RandomSource(0).Pick(new int[0]));
        }
    }
}
