using Domain.Models;
using Domain.Ports.Driving;
using System;
using System.Collections.Generic;

namespace Domain.Services
{
    // The game clock. It is fed real time, steps a fixed number of times per real second,
    // and advances game time by the current speed on each step. See docs/adr/game-clock.md.
    public class TimeService : ITimeService
    {
        private readonly int _maxStepsPerAdvance;
        private readonly float _stepLength;
        private readonly List<Hold> _holds = new List<Hold>();
        private bool _inRun;
        private float _pendingRealTime;

        public event Action<float> OnTick;

        public float TotalElapsedTime { get; private set; }
        public bool IsPaused => _inRun && _holds.Exists(hold => hold.IsPause);
        public float Speed => IsPaused ? 0f : SpeedIgnoringPauses();

        public TimeService(IGameStateService gameStateService)
            : this(gameStateService, ClockSettings.Default)
        {
        }

        public TimeService(IGameStateService gameStateService, ClockSettings settings)
        {
            _maxStepsPerAdvance = settings.MaxStepsPerAdvance;
            _stepLength = 1f / settings.StepsPerSecond;
            _inRun = gameStateService.CurrentState == GameState.Gameplay;
            gameStateService.OnStateChanged += OnGameStateChanged;
        }

        public void Advance(float realDeltaTime)
        {
            if (!_inRun) return;

            _pendingRealTime += realDeltaTime;
            int steps = 0;
            while (_pendingRealTime >= _stepLength)
            {
                if (steps == _maxStepsPerAdvance)
                {
                    _pendingRealTime = 0f;
                    break;
                }

                _pendingRealTime -= _stepLength;
                Step();
                steps++;

                if (!_inRun) return;
            }
        }

        public ITimeHold Pause(float? duration = null)
        {
            return AddHold(new Hold(this, isPause: true, 0f, 0f, 0f, duration));
        }

        public ITimeHold SlowTo(float speed, float rampDuration = 0f, float? duration = null)
        {
            if (speed < 0f || speed > 1f)
                throw new ArgumentOutOfRangeException(nameof(speed), speed, "A speed is between 0 and 1");

            return AddHold(new Hold(this, isPause: false, SpeedIgnoringPauses(), speed, rampDuration, duration));
        }

        private ITimeHold AddHold(Hold hold)
        {
            if (_inRun) _holds.Add(hold);
            else hold.Drop();
            return hold;
        }

        private void Step()
        {
            bool paused = IsPaused;
            float gameDelta = Speed * _stepLength;

            // Holds asked for during this step's tick start counting on the next step.
            Hold[] aging = _holds.ToArray();

            if (gameDelta > 0f)
            {
                TotalElapsedTime += gameDelta;
                OnTick?.Invoke(gameDelta);
            }

            foreach (Hold hold in aging)
            {
                hold.Age(_stepLength, paused);
            }
        }

        private float SpeedIgnoringPauses()
        {
            if (!_inRun) return 1f;

            float speed = 1f;
            foreach (Hold hold in _holds)
            {
                if (!hold.IsPause) speed = Math.Min(speed, hold.CurrentSpeed);
            }
            return speed;
        }

        private void OnGameStateChanged(GameState previousState, GameState newState)
        {
            foreach (Hold hold in _holds) hold.Drop();
            _holds.Clear();

            _inRun = newState == GameState.Gameplay;
            _pendingRealTime = 0f;
            if (_inRun) TotalElapsedTime = 0f;
        }

        private sealed class Hold : ITimeHold
        {
            // Absorbs float error so a length that is a whole number of steps ends on time.
            private const float Tolerance = 1e-5f;

            private readonly TimeService _clock;
            private readonly float _startSpeed;
            private readonly float _targetSpeed;
            private readonly float _rampDuration;
            private float _rampElapsed;
            private float? _remaining;
            private bool _active = true;

            public bool IsPause { get; }

            public float CurrentSpeed => _rampDuration <= 0f
                ? _targetSpeed
                : _startSpeed + (_targetSpeed - _startSpeed) * (_rampElapsed / _rampDuration);

            public Hold(TimeService clock, bool isPause, float startSpeed, float targetSpeed, float rampDuration, float? duration)
            {
                _clock = clock;
                IsPause = isPause;
                _startSpeed = startSpeed;
                _targetSpeed = targetSpeed;
                _rampDuration = rampDuration;
                _remaining = duration;
            }

            // Counts one step of real time. A pause counts while paused. A speed change
            // freezes while paused, so its ramp and length resume where they stopped.
            public void Age(float stepLength, bool paused)
            {
                if (!_active) return;
                if (!IsPause && paused) return;

                if (!IsPause) _rampElapsed = Math.Min(_rampElapsed + stepLength, _rampDuration);

                if (_remaining.HasValue)
                {
                    _remaining -= stepLength;
                    if (_remaining <= Tolerance) Release();
                }
            }

            public void Release()
            {
                if (!_active) return;
                _active = false;
                _clock._holds.Remove(this);
            }

            public void Drop()
            {
                _active = false;
            }
        }
    }
}
