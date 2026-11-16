using Domain.Models;
using Domain.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Domain.Tests
{
    // The clock under test steps 16 times per real second, so a step is 0.0625s,
    // which floats hold exactly, and runs at most 5 steps per call.
    [TestFixture]
    public class TimeServiceTests
    {
        private const float Step = 1f / 16f;
        private GameStateService _state;
        private TimeService _time;
        private List<float> _ticks;

        [SetUp]
        public void SetUp()
        {
            _state = new GameStateService();
            _time = new TimeService(_state, new ClockSettings(16, 5));
            _ticks = new List<float>();
            _time.OnTick += dt => _ticks.Add(dt);
            _state.TransitionTo(GameState.Gameplay);
        }

        [Test]
        public void Advance_FiresOneTickPerStep()
        {
            _time.Advance(3 * Step);

            CollectionAssert.AreEqual(new[] { Step, Step, Step }, _ticks);
        }

        [Test]
        public void Advance_CarriesLeftoverRealTimeToTheNextCall()
        {
            _time.Advance(0.75f * Step);
            CollectionAssert.IsEmpty(_ticks);

            _time.Advance(0.5f * Step);

            Assert.AreEqual(1, _ticks.Count);
        }

        [Test]
        public void Advance_AfterALongFrame_RunsAtMostTheCap_AndDropsTheRest()
        {
            _time.Advance(10f);
            Assert.AreEqual(5, _ticks.Count);

            _time.Advance(0.5f * Step);

            Assert.AreEqual(5, _ticks.Count);
        }

        [Test]
        public void OutsideARun_NoGameTimePasses_AtFullSpeed()
        {
            _state.TransitionTo(GameState.MainMenu);

            _time.Advance(1f);

            CollectionAssert.IsEmpty(_ticks);
            Assert.AreEqual(1f, _time.Speed);
            Assert.IsFalse(_time.IsPaused);
        }

        [Test]
        public void RequestsOutsideARun_HaveNoEffect()
        {
            _state.TransitionTo(GameState.MainMenu);
            _time.Pause();
            _time.SlowTo(0f);

            _state.TransitionTo(GameState.Gameplay);

            Assert.IsFalse(_time.IsPaused);
            Assert.AreEqual(1f, _time.Speed);
        }

        [Test]
        public void Pause_StopsGameTime_UntilReleased()
        {
            var pause = _time.Pause();
            _time.Advance(Step);

            CollectionAssert.IsEmpty(_ticks);
            Assert.IsTrue(_time.IsPaused);
            Assert.AreEqual(0f, _time.Speed);

            pause.Release();
            _time.Advance(Step);

            Assert.AreEqual(1, _ticks.Count);
        }

        [Test]
        public void ReleasingOnePause_LeavesAnotherInForce()
        {
            var hitStop = _time.Pause();
            _time.Pause();

            hitStop.Release();
            hitStop.Release();

            Assert.IsTrue(_time.IsPaused);
        }

        [Test]
        public void SlowestSpeedWins()
        {
            _time.SlowTo(0.5f);
            var slower = _time.SlowTo(0.25f);
            Assert.AreEqual(0.25f, _time.Speed);

            slower.Release();

            Assert.AreEqual(0.5f, _time.Speed);
        }

        [Test]
        public void PauseBeatsAnySpeed()
        {
            _time.SlowTo(0.5f);
            _time.Pause();

            Assert.AreEqual(0f, _time.Speed);
        }

        [Test]
        public void SlowMotion_KeepsTheStepRate_AndShrinksEachStep()
        {
            _time.SlowTo(0.5f);

            _time.Advance(2 * Step);

            CollectionAssert.AreEqual(new[] { Step / 2f, Step / 2f }, _ticks);
        }

        [Test]
        public void AtSpeedZero_NoTicksFire_ButTheRunIsNotPaused()
        {
            _time.SlowTo(0f);

            _time.Advance(4 * Step);

            CollectionAssert.IsEmpty(_ticks);
            Assert.IsFalse(_time.IsPaused);
        }

        [Test]
        public void Ramp_ChangesSpeedOverRealTime()
        {
            _time.SlowTo(0f, rampDuration: 8 * Step);

            _time.Advance(4 * Step);
            Assert.AreEqual(0.5f, _time.Speed, 1e-5f);

            _time.Advance(4 * Step);
            Assert.AreEqual(0f, _time.Speed, 1e-5f);
        }

        [Test]
        public void Ramp_FreezesWhilePaused()
        {
            _time.SlowTo(0f, rampDuration: 8 * Step);
            _time.Advance(4 * Step);

            var pause = _time.Pause();
            _time.Advance(5 * Step);
            pause.Release();

            Assert.AreEqual(0.5f, _time.Speed, 1e-5f);
        }

        [Test]
        public void TimedPause_ReleasesItselfAfterItsLength()
        {
            _time.Pause(4 * Step);

            _time.Advance(4 * Step);
            CollectionAssert.IsEmpty(_ticks);
            Assert.IsFalse(_time.IsPaused);

            _time.Advance(Step);
            Assert.AreEqual(1, _ticks.Count);
        }

        [Test]
        public void TimedSlowMotion_DoesNotCountDownWhilePaused()
        {
            _time.SlowTo(0.5f, duration: 2 * Step);
            var pause = _time.Pause();
            _time.Advance(5 * Step);
            pause.Release();
            Assert.AreEqual(0.5f, _time.Speed);

            _time.Advance(2 * Step);

            Assert.AreEqual(1f, _time.Speed);
        }

        [Test]
        public void HoldRequestedDuringATick_StartsCountingOnTheNextStep()
        {
            bool first = true;
            _time.OnTick += dt =>
            {
                if (!first) return;
                first = false;
                _time.Pause(2 * Step);
            };

            _time.Advance(4 * Step);

            // Step 1 ticks and asks for the pause, steps 2 and 3 are paused, step 4 ticks.
            Assert.AreEqual(2, _ticks.Count);
        }

        [Test]
        public void EndingARun_DropsEveryHold_AndRestoresFullSpeed()
        {
            var death = _time.SlowTo(0f);

            _state.TransitionTo(GameState.MainMenu);
            Assert.AreEqual(1f, _time.Speed);

            _state.TransitionTo(GameState.Gameplay);
            _time.SlowTo(0.5f);
            death.Release();

            Assert.AreEqual(0.5f, _time.Speed);
            _time.Advance(Step);
            Assert.AreEqual(1, _ticks.Count);
        }

        [Test]
        public void TotalElapsedTime_CountsGameTime_FromTheStartOfTheRun()
        {
            _time.Advance(2 * Step);
            _time.SlowTo(0.5f);
            _time.Advance(2 * Step);
            Assert.AreEqual(3 * Step, _time.TotalElapsedTime, 1e-6f);

            _state.TransitionTo(GameState.MainMenu);
            _state.TransitionTo(GameState.Gameplay);

            Assert.AreEqual(0f, _time.TotalElapsedTime);
        }

        [Test]
        public void SlowTo_OutsideZeroToOne_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _time.SlowTo(1.5f));
            Assert.Throws<ArgumentOutOfRangeException>(() => _time.SlowTo(-0.1f));
        }
    }
}
