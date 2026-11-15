using NUnit.Framework;
using System.Linq;

namespace Tests
{
    [TestFixture]
    public class ArenaTests
    {
        [Test]
        public void Constructor_InitializesProperties()
        {
            var arena = new RectangleArena(5, 7, new System.Random(0));

            Assert.AreEqual(5, arena.Width);
            Assert.AreEqual(7, arena.Height);
        }

        [Test]
        public void Constructor_InitializesAllTilesAsFloor()
        {
            var arena = new RectangleArena(3, 4, new System.Random(0));

            var allTiles = arena.GetAllTiles().ToList();

            Assert.AreEqual(12, allTiles.Count);
            Assert.IsTrue(allTiles.All(tile => tile.tileType == TileType.Floor));
        }

        #region IsInBounds Tests

        [Test]
        public void IsInBounds_ReturnsTrueForValidPosition()
        {
            var arena = new RectangleArena(5, 5, new System.Random(0));

            Assert.IsTrue(arena.IsInBounds(new GridPosition(2, 2)));
            Assert.IsTrue(arena.IsInBounds(new GridPosition(0, 0)));
            Assert.IsTrue(arena.IsInBounds(new GridPosition(4, 4)));
        }

        [Test]
        public void IsInBounds_ReturnsTrueForAllCorners()
        {
            var arena = new RectangleArena(3, 4, new System.Random(0));

            Assert.IsTrue(arena.IsInBounds(new GridPosition(0, 0)), "Bottom-left corner");
            Assert.IsTrue(arena.IsInBounds(new GridPosition(2, 0)), "Bottom-right corner");
            Assert.IsTrue(arena.IsInBounds(new GridPosition(0, 3)), "Top-left corner");
            Assert.IsTrue(arena.IsInBounds(new GridPosition(2, 3)), "Top-right corner");
        }

        [Test]
        public void IsInBounds_ReturnsFalseForNegativeX()
        {
            var arena = new RectangleArena(5, 5, new System.Random(0));

            Assert.IsFalse(arena.IsInBounds(new GridPosition(-1, 2)));
            Assert.IsFalse(arena.IsInBounds(new GridPosition(-10, 0)));
        }

        [Test]
        public void IsInBounds_ReturnsFalseForNegativeY()
        {
            var arena = new RectangleArena(5, 5, new System.Random(0));

            Assert.IsFalse(arena.IsInBounds(new GridPosition(2, -1)));
            Assert.IsFalse(arena.IsInBounds(new GridPosition(0, -5)));
        }

        [Test]
        public void IsInBounds_ReturnsFalseForXBeyondWidth()
        {
            var arena = new RectangleArena(5, 5, new System.Random(0));

            Assert.IsFalse(arena.IsInBounds(new GridPosition(5, 2)));
            Assert.IsFalse(arena.IsInBounds(new GridPosition(10, 0)));
        }

        [Test]
        public void IsInBounds_ReturnsFalseForYBeyondHeight()
        {
            var arena = new RectangleArena(5, 5, new System.Random(0));

            Assert.IsFalse(arena.IsInBounds(new GridPosition(2, 5)));
            Assert.IsFalse(arena.IsInBounds(new GridPosition(0, 10)));
        }

        [Test]
        public void IsInBounds_ReturnsFalseForBothOutOfBounds()
        {
            var arena = new RectangleArena(5, 5, new System.Random(0));

            Assert.IsFalse(arena.IsInBounds(new GridPosition(-1, -1)));
            Assert.IsFalse(arena.IsInBounds(new GridPosition(5, 5)));
            Assert.IsFalse(arena.IsInBounds(new GridPosition(10, 10)));
        }

        #endregion

        #region GetTileType Tests

        [Test]
        public void GetTileType_ReturnsFloorForValidPositions()
        {
            var arena = new RectangleArena(5, 5, new System.Random(0));

            Assert.AreEqual(TileType.Floor, arena.GetTileType(new GridPosition(0, 0)));
            Assert.AreEqual(TileType.Floor, arena.GetTileType(new GridPosition(2, 2)));
            Assert.AreEqual(TileType.Floor, arena.GetTileType(new GridPosition(4, 4)));
        }

        [Test]
        public void GetTileType_ReturnsEmptyForOutOfBounds()
        {
            var arena = new RectangleArena(5, 5, new System.Random(0));

            Assert.AreEqual(TileType.Empty, arena.GetTileType(new GridPosition(-1, 0)));
            Assert.AreEqual(TileType.Empty, arena.GetTileType(new GridPosition(0, -1)));
            Assert.AreEqual(TileType.Empty, arena.GetTileType(new GridPosition(5, 0)));
            Assert.AreEqual(TileType.Empty, arena.GetTileType(new GridPosition(0, 5)));
            Assert.AreEqual(TileType.Empty, arena.GetTileType(new GridPosition(10, 10)));
        }

        #endregion

        #region GetAllTiles Tests

        [Test]
        public void GetAllTiles_ReturnsCorrectCount()
        {
            var arena = new RectangleArena(3, 4, new System.Random(0));

            var tiles = arena.GetAllTiles().ToList();

            Assert.AreEqual(12, tiles.Count);
        }

        [Test]
        public void GetAllTiles_ReturnsAllPositions()
        {
            var arena = new RectangleArena(3, 2, new System.Random(0));

            var tiles = arena.GetAllTiles().ToList();
            var positions = tiles.Select(t => t.position).ToList();

            Assert.IsTrue(positions.Contains(new GridPosition(0, 0)));
            Assert.IsTrue(positions.Contains(new GridPosition(1, 0)));
            Assert.IsTrue(positions.Contains(new GridPosition(2, 0)));
            Assert.IsTrue(positions.Contains(new GridPosition(0, 1)));
            Assert.IsTrue(positions.Contains(new GridPosition(1, 1)));
            Assert.IsTrue(positions.Contains(new GridPosition(2, 1)));
        }

        [Test]
        public void GetAllTiles_AllTilesAreFloor()
        {
            var arena = new RectangleArena(5, 5, new System.Random(0));

            var tiles = arena.GetAllTiles().ToList();

            Assert.IsTrue(tiles.All(t => t.tileType == TileType.Floor));
        }

        [Test]
        public void GetAllTiles_CoversEntireArenaGrid()
        {
            var arena = new RectangleArena(4, 3, new System.Random(0));

            var tiles = arena.GetAllTiles().ToList();

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    var expectedPos = new GridPosition(x, y);
                    Assert.IsTrue(
                        tiles.Any(t => t.position.Equals(expectedPos)),
                        $"Missing position ({x}, {y})"
                    );
                }
            }
        }

        #endregion

        #region Edge Case Tests

        [Test]
        public void Arena_WithSingleTile_WorksCorrectly()
        {
            var arena = new RectangleArena(1, 1, new System.Random(0));

            Assert.AreEqual(1, arena.Width);
            Assert.AreEqual(1, arena.Height);
            Assert.IsTrue(arena.IsInBounds(new GridPosition(0, 0)));
            Assert.IsFalse(arena.IsInBounds(new GridPosition(1, 0)));
            Assert.IsFalse(arena.IsInBounds(new GridPosition(0, 1)));
            Assert.AreEqual(1, arena.GetAllTiles().Count());
        }

        [Test]
        public void Arena_WithAsymmetricDimensions_WorksCorrectly()
        {
            var arena = new RectangleArena(2, 5, new System.Random(0));

            Assert.IsTrue(arena.IsInBounds(new GridPosition(1, 4)));
            Assert.IsFalse(arena.IsInBounds(new GridPosition(2, 4)));
            Assert.IsFalse(arena.IsInBounds(new GridPosition(1, 5)));
            Assert.AreEqual(10, arena.GetAllTiles().Count());
        }


        #endregion
    }
}