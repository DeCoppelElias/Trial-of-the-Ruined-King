using System;
using System.Collections.Generic;

namespace Domain.Models
{
    // The one way the game rolls. See docs/adr/randomness.md.
    public sealed class RandomSource
    {
        private Random _random;

        public int Seed { get; private set; }

        public RandomSource(int seed)
        {
            Reseed(seed);
        }

        public static int NewSeed()
        {
            return Guid.NewGuid().GetHashCode();
        }

        public void Reseed(int seed)
        {
            Seed = seed;
            _random = new Random(seed);
        }

        public int Range(int minInclusive, int maxExclusive)
        {
            return _random.Next(minInclusive, maxExclusive);
        }

        public float Range(float min, float max)
        {
            return min + (float)_random.NextDouble() * (max - min);
        }

        public float Value()
        {
            return (float)_random.NextDouble();
        }

        public bool CoinFlip()
        {
            return _random.Next(2) == 0;
        }

        public T Pick<T>(IReadOnlyList<T> items)
        {
            if (items == null || items.Count == 0)
                throw new ArgumentException("Cannot pick from an empty list.", nameof(items));

            return items[_random.Next(items.Count)];
        }
    }
}
