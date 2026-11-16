using System;

namespace Domain.Models
{
    // How the game clock steps. Default is the one place the game's step rate is set.
    public readonly struct ClockSettings
    {
        public static readonly ClockSettings Default = new ClockSettings(60, 5);

        // Steps per second of real time.
        public int StepsPerSecond { get; }

        // The most steps one call to Advance may run. Real time beyond that is dropped,
        // so a long frame slows the game briefly instead of running a burst of steps.
        public int MaxStepsPerAdvance { get; }

        public ClockSettings(int stepsPerSecond, int maxStepsPerAdvance)
        {
            if (stepsPerSecond <= 0) throw new ArgumentOutOfRangeException(nameof(stepsPerSecond));
            if (maxStepsPerAdvance <= 0) throw new ArgumentOutOfRangeException(nameof(maxStepsPerAdvance));

            StepsPerSecond = stepsPerSecond;
            MaxStepsPerAdvance = maxStepsPerAdvance;
        }
    }
}
