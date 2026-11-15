using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class GridDirectionTests
    {
        [Test]
        public void ApplyTo_MovesPositionByDelta()
        {
            var moved = GridDirection.Up.ApplyTo(new GridPosition(2, 3));

            Assert.AreEqual(new GridPosition(2, 4), moved);
        }

        [Test]
        public void Directions_HaveExpectedDeltas()
        {
            Assert.AreEqual((1, 0), (GridDirection.Right.DeltaX, GridDirection.Right.DeltaY));
            Assert.AreEqual((-1, 0), (GridDirection.Left.DeltaX, GridDirection.Left.DeltaY));
            Assert.AreEqual((0, 1), (GridDirection.Up.DeltaX, GridDirection.Up.DeltaY));
            Assert.AreEqual((0, -1), (GridDirection.Down.DeltaX, GridDirection.Down.DeltaY));
        }
    }
}
