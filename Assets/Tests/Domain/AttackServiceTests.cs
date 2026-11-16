using Domain.Models;
using Domain.Services;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Tests
{
    [TestFixture]
    public class AttackServiceTests
    {
        private RectangleArena _arena;
        private GameStateService _state;
        private TimeService _time;
        private AttackService _attacks;

        [SetUp]
        public void SetUp()
        {
            _arena = new RectangleArena(3, 4, new System.Random(0));
            _state = new GameStateService();
            _time = TestClock.Create(_state);
            _attacks = new AttackService(_time, _state);
            _state.TransitionTo(GameState.Gameplay);
        }

        // Each stage lasts one second.
        private HammerAttack Hammer(int x = 1, int y = 1)
        {
            return new HammerAttack(new GridPosition(x, y), HorizontalDirection.LeftToRight,
                new AttackTiming(1f, 1f, 1f, 1f), _arena);
        }

        [Test]
        public void SpawnAttack_AddsAndStartsTheAttack_AndReportsIt()
        {
            var hammer = Hammer();
            var added = new List<Attack>();
            _attacks.OnAttackAdded += added.Add;

            _attacks.SpawnAttack(hammer);

            CollectionAssert.AreEqual(new[] { hammer }, _attacks.GetActiveAttacks().ToList());
            CollectionAssert.AreEqual(new[] { hammer }, added);
            Assert.AreEqual(AttackStage.Telegraph, hammer.CurrentStage);
        }

        [Test]
        public void Ticking_AdvancesActiveAttacks()
        {
            var hammer = Hammer();
            _attacks.SpawnAttack(hammer);

            _time.Advance(1f);

            Assert.AreEqual(AttackStage.Commit, hammer.CurrentStage);
        }

        [Test]
        public void FinishedAttack_IsRemoved_AndReported()
        {
            var hammer = Hammer();
            var completed = new List<Attack>();
            _attacks.OnAttackCompleted += completed.Add;
            _attacks.SpawnAttack(hammer);

            for (int i = 0; i < 4; i++) _time.Advance(1f);

            CollectionAssert.IsEmpty(_attacks.GetActiveAttacks());
            CollectionAssert.AreEqual(new[] { hammer }, completed);
        }

        [Test]
        public void CheckPlayerHit_IsFalse_WhenNoAttackEndangersThePlayer()
        {
            _attacks.SpawnAttack(Hammer());

            Assert.IsFalse(_attacks.CheckPlayerHit(new GridPosition(1, 1)), "telegraph does not hit");
        }

        [Test]
        public void CheckPlayerHit_HitsOncePerAttack()
        {
            var hammer = Hammer();
            _attacks.SpawnAttack(hammer);
            _time.Advance(1f);
            _time.Advance(1f);

            Assert.IsTrue(_attacks.CheckPlayerHit(new GridPosition(1, 1)));
            Assert.IsTrue(hammer.HasDealtDamage);
            Assert.IsFalse(_attacks.CheckPlayerHit(new GridPosition(1, 1)));
        }

        [Test]
        public void CheckPlayerHit_EachAttackHitsSeparately()
        {
            _attacks.SpawnAttack(Hammer());
            _attacks.SpawnAttack(Hammer());
            _time.Advance(1f);
            _time.Advance(1f);

            Assert.IsTrue(_attacks.CheckPlayerHit(new GridPosition(1, 1)));
            Assert.IsTrue(_attacks.CheckPlayerHit(new GridPosition(1, 1)));
            Assert.IsFalse(_attacks.CheckPlayerHit(new GridPosition(1, 1)));
        }

        [Test]
        public void ReturningToMainMenu_ClearsEveryAttack()
        {
            _attacks.SpawnAttack(Hammer());
            _attacks.SpawnAttack(Hammer(0, 0));

            _state.TransitionTo(GameState.MainMenu);

            CollectionAssert.IsEmpty(_attacks.GetActiveAttacks());
        }
    }
}
