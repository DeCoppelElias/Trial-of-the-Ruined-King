using Domain.Models;
using NUnit.Framework;

namespace Domain.Tests
{
    [TestFixture]
    public class PlayerTests
    {
        private readonly GridPosition _start = new GridPosition(1, 1);

        [Test]
        public void NewPlayer_IsAliveAtTheStart_WithFullHealth()
        {
            var player = new Player(3, _start);

            Assert.AreEqual(PlayerState.Alive, player.State);
            Assert.AreEqual(3, player.Health);
            Assert.AreEqual(_start, player.GridPosition);
        }

        [TestCase(0)]
        [TestCase(-2)]
        public void InitialHealth_BelowOne_BecomesOne(int initialHealth)
        {
            Assert.AreEqual(1, new Player(initialHealth, _start).Health);
        }

        [Test]
        public void TakeDamage_LowersHealth()
        {
            var player = new Player(3, _start);

            player.TakeDamage(1);

            Assert.AreEqual(2, player.Health);
            Assert.AreEqual(PlayerState.Alive, player.State);
        }

        [Test]
        public void TakeDamage_ToZero_Kills()
        {
            var player = new Player(1, _start);

            player.TakeDamage(1);

            Assert.AreEqual(PlayerState.Dead, player.State);
        }

        [Test]
        public void Health_NeverGoesBelowZero()
        {
            var player = new Player(1, _start);

            player.TakeDamage(5);

            Assert.AreEqual(0, player.Health);
        }

        [Test]
        public void NegativeDamage_IsIgnored()
        {
            var player = new Player(2, _start);

            player.TakeDamage(-1);

            Assert.AreEqual(2, player.Health);
        }

        [Test]
        public void DeadPlayer_TakesNoMoreDamage()
        {
            var player = new Player(1, _start);
            player.TakeDamage(1);

            player.TakeDamage(1);

            Assert.AreEqual(0, player.Health);
            Assert.AreEqual(PlayerState.Dead, player.State);
        }

        [Test]
        public void Move_ChangesPosition()
        {
            var player = new Player(1, _start);

            player.Move(new GridPosition(2, 3));

            Assert.AreEqual(new GridPosition(2, 3), player.GridPosition);
        }

        [Test]
        public void Reset_RestoresHealthLifeAndPosition()
        {
            var player = new Player(2, _start);
            player.Move(new GridPosition(0, 0));
            player.TakeDamage(2);

            player.Reset();

            Assert.AreEqual(2, player.Health);
            Assert.AreEqual(PlayerState.Alive, player.State);
            Assert.AreEqual(_start, player.GridPosition);
        }
    }
}
