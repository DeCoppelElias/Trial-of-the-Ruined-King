using System;

namespace Domain.Ports.Driving
{
    // The game clock. See docs/adr/game-clock.md.
    public interface ITimeService
    {
        // Fires once per step that advances game time, with the game seconds advanced.
        event Action<float> OnTick;

        // Feeds the clock real seconds. Called once per frame by one adapter.
        void Advance(float realDeltaTime);

        // How fast game time runs against real time: 0 while paused, 1 outside a run.
        float Speed { get; }

        bool IsPaused { get; }

        // Game seconds since the run started.
        float TotalElapsedTime { get; }

        // Pauses the run. With a duration, in real seconds, the clock releases the hold itself.
        ITimeHold Pause(float? duration = null);

        // Asks for game time to run at speed, between 0 and 1, reached over rampDuration
        // real seconds. With a duration, in real seconds, the clock releases the hold itself.
        ITimeHold SlowTo(float speed, float rampDuration = 0f, float? duration = null);
    }
}
