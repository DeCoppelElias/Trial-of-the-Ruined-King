using Domain.Models;
using Domain.Services;

namespace Domain.Tests
{
    // A game clock for tests that pass time in whole seconds. It steps 16 times per real
    // second, which floats hold exactly, and never caps the steps in one call.
    internal static class TestClock
    {
        public static TimeService Create(GameStateService state)
        {
            return new TimeService(state, new ClockSettings(16, int.MaxValue));
        }
    }
}
