using NUnit.Framework;
using System.Linq;
using UnityEngine;

namespace Tests
{
    [TestFixture]
    public class ArenaTests
    {
        [Test]
        public void Constructor_InitializesProperties()
        {
            var worldPos = new Vector3(10f, 5f, -3f);
            var arena = new RectangleArena(5, 7, worldPos);

            Assert.AreEqual(5, arena.Width);
            Assert.AreEqual(7, arena.Height);
            Assert.AreEqual(worldPos, arena.WorldPosition);
        }

        [Test]
        public void Constructor_InitializesAllTilesAsFloor()
        {
            var arena = new RectangleArena(3, 4, Vector3.zero);

            var allTiles = arena.GetAllTiles().ToList();

            Assert.AreEqual(12, allTiles.Count);
            Assert.IsTrue(allTiles.All(tile => tile.tileType == TileType.Floor));
        }

        #region IsInBounds Tests

        [Test]
        public void IsInBounds_ReturnsTrueForValidPosition()
        {
            var arena = new RectangleArena(5, 5, Vector3.zero);

            Assert.IsTrue(arena.IsInBounds(new GridPosition(2, 2)));
            Assert.IsTrue(arena.IsInBounds(new GridPosition(0, 0)));
            Assert.IsTrue(arena.IsInBounds(new GridPosition(4, 4)));
        }

        [Test]
        public void IsInBounds_ReturnsTrueForAllCorners()
        {
            var arena = new RectangleArena(3, 4, Vector3.zero);

            Assert.IsTrue(arena.IsInBounds(new GridPosition(0, 0)), "Bottom-left corner");
            Assert.IsTrue(arena.IsInBounds(new GridPosition(2, 0)), "Bottom-right corner");
            Assert.IsTrue(arena.IsInBounds(new GridPosition(0, 3)), "Top-left corner");
            Assert.IsTrue(arena.IsInBounds(new GridPosition(2, 3)), "Top-right corner");
        }

        [Test]
        public void IsInBounds_ReturnsFalseForNegativeX()
        {
            var arena = new RectangleArena(5, 5, Vector3.zero);

            Assert.IsFalse(arena.IsInBounds(new GridPosition(-1, 2)));
            Assert.IsFalse(arena.IsInBounds(new GridPosition(-10, 0)));
        }

        [Test]
        public void IsInBounds_ReturnsFalseForNegativeY()
        {
            var arena = new RectangleArena(5, 5, Vector3.zero);

            Assert.IsFalse(arena.IsInBounds(new GridPosition(2, -1)));
            Assert.IsFalse(arena.IsInBounds(new GridPosition(0, -5)));
        }

        [Test]
        public void IsInBounds_ReturnsFalseForXBeyondWidth()
        {
            var arena = new RectangleArena(5, 5, Vector3.zero);

            Assert.IsFalse(arena.IsInBounds(new GridPosition(5, 2)));
            Assert.IsFalse(arena.IsInBounds(new GridPosition(10, 0)));
        }

        [Test]
        public void IsInBounds_ReturnsFalseForYBeyondHeight()
        {
            var arena = new RectangleArena(5, 5, Vector3.zero);

            Assert.IsFalse(arena.IsInBounds(new GridPosition(2, 5)));
            Assert.IsFalse(arena.IsInBounds(new GridPosition(0, 10)));
        }

        [Test]
        public void IsInBounds_ReturnsFalseForBothOutOfBounds()
        {
            var arena = new RectangleArena(5, 5, Vector3.zero);

            Assert.IsFalse(arena.IsInBounds(new GridPosition(-1, -1)));
            Assert.IsFalse(arena.IsInBounds(new GridPosition(5, 5)));
            Assert.IsFalse(arena.IsInBounds(new GridPosition(10, 10)));
        }

        #endregion

        #region GridToWorld Tests

        [Test]
        public void GridToWorld_WithZeroWorldPosition_ReturnsCorrectCoordinates()
        {
            var arena = new RectangleArena(5, 5, Vector3.zero);

            var worldPos = arena.GridToWorld(new GridPosition(2, 3));

            Assert.AreEqual(new Vector3(2, 0, 3), worldPos);
        }

        [Test]
        public void GridToWorld_WithNonZeroWorldPosition_AppliesOffset()
        {
            var arena = new RectangleArena(5, 5, new Vector3(10f, 5f, -3f));

            var worldPos = arena.GridToWorld(new GridPosition(2, 3));

            Assert.AreEqual(new Vector3(12f, 5f, 0f), worldPos);
        }

        [Test]
        public void GridToWorld_Origin_ReturnsWorldPosition()
        {
            var worldPosition = new Vector3(-5f, 2f, 7f);
            var arena = new RectangleArena(5, 5, worldPosition);

            var worldPos = arena.GridToWorld(new GridPosition(0, 0));

            Assert.AreEqual(worldPosition, worldPos);
        }

        [Test]
        public void GridToWorld_AlwaysHasZeroYComponent()
        {
            var arena = new RectangleArena(5, 5, new Vector3(1f, 99f, 2f));

            var worldPos1 = arena.GridToWorld(new GridPosition(0, 0));
            var worldPos2 = arena.GridToWorld(new GridPosition(3, 4));

            Assert.AreEqual(99f, worldPos1.y);
            Assert.AreEqual(99f, worldPos2.y);
        }

        #endregion

        #region WorldToGrid Tests

        [Test]
        public void WorldToGrid_WithZeroWorldPosition_ReturnsCorrectGrid()
        {
            var arena = new RectangleArena(5, 5, Vector3.zero);

            var gridPos = arena.WorldToGrid(new Vector3(2f, 0f, 3f));

            Assert.AreEqual(new GridPosition(2, 3), gridPos);
        }

        [Test]
        public void WorldToGrid_WithNonZeroWorldPosition_SubtractsOffset()
        {
            var arena = new RectangleArena(5, 5, new Vector3(10f, 5f, -3f));

            var gridPos = arena.WorldToGrid(new Vector3(12f, 5f, 0f));

            Assert.AreEqual(new GridPosition(2, 3), gridPos);
        }

        [Test]
        public void WorldToGrid_FloorsDecimalValues()
        {
            var arena = new RectangleArena(5, 5, Vector3.zero);

            var gridPos1 = arena.WorldToGrid(new Vector3(2.1f, 0f, 3.9f));
            var gridPos2 = arena.WorldToGrid(new Vector3(2.99f, 0f, 3.01f));

            Assert.AreEqual(new GridPosition(2, 3), gridPos1);
            Assert.AreEqual(new GridPosition(2, 3), gridPos2);
        }

        [Test]
        public void WorldToGrid_HandlesNegativeCoordinates()
        {
            var arena = new RectangleArena(5, 5, Vector3.zero);

            var gridPos1 = arena.WorldToGrid(new Vector3(-1f, 0f, -1f));
            var gridPos2 = arena.WorldToGrid(new Vector3(-0.5f, 0f, -0.1f));

            Assert.AreEqual(new GridPosition(-1, -1), gridPos1);
            Assert.AreEqual(new GridPosition(-1, -1), gridPos2);
        }

        [Test]
        public void WorldToGrid_IgnoresYComponent()
        {
            var arena = new RectangleArena(5, 5, Vector3.zero);

            var gridPos1 = arena.WorldToGrid(new Vector3(2f, 0f, 3f));
            var gridPos2 = arena.WorldToGrid(new Vector3(2f, 100f, 3f));
            var gridPos3 = arena.WorldToGrid(new Vector3(2f, -50f, 3f));

            Assert.AreEqual(new GridPosition(2, 3), gridPos1);
            Assert.AreEqual(new GridPosition(2, 3), gridPos2);
            Assert.AreEqual(new GridPosition(2, 3), gridPos3);
        }

        #endregion

        #region Grid↔World Round-Trip Tests

        [Test]
        public void GridToWorld_ThenWorldToGrid_ReturnsOriginalPosition()
        {
            var arena = new RectangleArena(5, 5, new Vector3(10f, 0f, -5f));
            var originalGrid = new GridPosition(3, 2);

            var worldPos = arena.GridToWorld(originalGrid);
            var resultGrid = arena.WorldToGrid(worldPos);

            Assert.AreEqual(originalGrid, resultGrid);
        }

        [Test]
        public void GridToWorld_ThenWorldToGrid_WorksForAllValidPositions()
        {
            var arena = new RectangleArena(3, 4, new Vector3(5f, 0f, 5f));

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    var originalGrid = new GridPosition(x, y);
                    var worldPos = arena.GridToWorld(originalGrid);
                    var resultGrid = arena.WorldToGrid(worldPos);

                    Assert.AreEqual(originalGrid, resultGrid, $"Failed for position ({x}, {y})");
                }
            }
        }

        #endregion

        #region GetTileType Tests

        [Test]
        public void GetTileType_ReturnsFloorForValidPositions()
        {
            var arena = new RectangleArena(5, 5, Vector3.zero);

            Assert.AreEqual(TileType.Floor, arena.GetTileType(new GridPosition(0, 0)));
            Assert.AreEqual(TileType.Floor, arena.GetTileType(new GridPosition(2, 2)));
            Assert.AreEqual(TileType.Floor, arena.GetTileType(new GridPosition(4, 4)));
        }

        [Test]
        public void GetTileType_ReturnsEmptyForOutOfBounds()
        {
            var arena = new RectangleArena(5, 5, Vector3.zero);

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
            var arena = new RectangleArena(3, 4, Vector3.zero);

            var tiles = arena.GetAllTiles().ToList();

            Assert.AreEqual(12, tiles.Count);
        }

        [Test]
        public void GetAllTiles_ReturnsAllPositions()
        {
            var arena = new RectangleArena(3, 2, Vector3.zero);

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
            var arena = new RectangleArena(5, 5, Vector3.zero);

            var tiles = arena.GetAllTiles().ToList();

            Assert.IsTrue(tiles.All(t => t.tileType == TileType.Floor));
        }

        [Test]
        public void GetAllTiles_CoversEntireArenaGrid()
        {
            var arena = new RectangleArena(4, 3, Vector3.zero);

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
            var arena = new RectangleArena(1, 1, Vector3.zero);

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
            var arena = new RectangleArena(2, 5, Vector3.zero);

            Assert.IsTrue(arena.IsInBounds(new GridPosition(1, 4)));
            Assert.IsFalse(arena.IsInBounds(new GridPosition(2, 4)));
            Assert.IsFalse(arena.IsInBounds(new GridPosition(1, 5)));
            Assert.AreEqual(10, arena.GetAllTiles().Count());
        }

        [Test]
        public void GridToWorld_WithNegativeWorldPosition_WorksCorrectly()
        {
            var arena = new RectangleArena(5, 5, new Vector3(-10f, 0f, -10f));

            var worldPos = arena.GridToWorld(new GridPosition(2, 3));

            Assert.AreEqual(new Vector3(-8f, 0f, -7f), worldPos);
        }

        #endregion
    }
}