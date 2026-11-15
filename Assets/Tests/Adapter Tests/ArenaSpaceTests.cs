using Domain.Models;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Adapters
{
    [TestFixture]
    public class ArenaSpaceTests
    {
        [Test]
        public void Constructor_StoresOrigin()
        {
            var origin = new Vector3(10f, 5f, -3f);
            var space = new ArenaSpace(origin);

            Assert.AreEqual(origin, space.Origin);
        }

        #region GridToWorld Tests

        [Test]
        public void GridToWorld_WithZeroOrigin_ReturnsCorrectCoordinates()
        {
            var space = new ArenaSpace(Vector3.zero);

            var worldPos = space.GridToWorld(new GridPosition(2, 3));

            Assert.AreEqual(new Vector3(2, 0, 3), worldPos);
        }

        [Test]
        public void GridToWorld_WithNonZeroOrigin_AppliesOffset()
        {
            var space = new ArenaSpace(new Vector3(10f, 5f, -3f));

            var worldPos = space.GridToWorld(new GridPosition(2, 3));

            Assert.AreEqual(new Vector3(12f, 5f, 0f), worldPos);
        }

        [Test]
        public void GridToWorld_Origin_ReturnsOrigin()
        {
            var worldPosition = new Vector3(-5f, 2f, 7f);
            var space = new ArenaSpace(worldPosition);

            var worldPos = space.GridToWorld(new GridPosition(0, 0));

            Assert.AreEqual(worldPosition, worldPos);
        }

        [Test]
        public void GridToWorld_KeepsOriginHeight()
        {
            var space = new ArenaSpace(new Vector3(1f, 99f, 2f));

            var worldPos1 = space.GridToWorld(new GridPosition(0, 0));
            var worldPos2 = space.GridToWorld(new GridPosition(3, 4));

            Assert.AreEqual(99f, worldPos1.y);
            Assert.AreEqual(99f, worldPos2.y);
        }

        #endregion

        #region WorldToGrid Tests

        [Test]
        public void WorldToGrid_WithZeroOrigin_ReturnsCorrectGrid()
        {
            var space = new ArenaSpace(Vector3.zero);

            var gridPos = space.WorldToGrid(new Vector3(2f, 0f, 3f));

            Assert.AreEqual(new GridPosition(2, 3), gridPos);
        }

        [Test]
        public void WorldToGrid_WithNonZeroOrigin_SubtractsOffset()
        {
            var space = new ArenaSpace(new Vector3(10f, 5f, -3f));

            var gridPos = space.WorldToGrid(new Vector3(12f, 5f, 0f));

            Assert.AreEqual(new GridPosition(2, 3), gridPos);
        }

        [Test]
        public void WorldToGrid_FloorsDecimalValues()
        {
            var space = new ArenaSpace(Vector3.zero);

            var gridPos1 = space.WorldToGrid(new Vector3(2.1f, 0f, 3.9f));
            var gridPos2 = space.WorldToGrid(new Vector3(2.99f, 0f, 3.01f));

            Assert.AreEqual(new GridPosition(2, 3), gridPos1);
            Assert.AreEqual(new GridPosition(2, 3), gridPos2);
        }

        [Test]
        public void WorldToGrid_HandlesNegativeCoordinates()
        {
            var space = new ArenaSpace(Vector3.zero);

            var gridPos1 = space.WorldToGrid(new Vector3(-1f, 0f, -1f));
            var gridPos2 = space.WorldToGrid(new Vector3(-0.5f, 0f, -0.1f));

            Assert.AreEqual(new GridPosition(-1, -1), gridPos1);
            Assert.AreEqual(new GridPosition(-1, -1), gridPos2);
        }

        [Test]
        public void WorldToGrid_IgnoresYComponent()
        {
            var space = new ArenaSpace(Vector3.zero);

            var gridPos1 = space.WorldToGrid(new Vector3(2f, 0f, 3f));
            var gridPos2 = space.WorldToGrid(new Vector3(2f, 100f, 3f));
            var gridPos3 = space.WorldToGrid(new Vector3(2f, -50f, 3f));

            Assert.AreEqual(new GridPosition(2, 3), gridPos1);
            Assert.AreEqual(new GridPosition(2, 3), gridPos2);
            Assert.AreEqual(new GridPosition(2, 3), gridPos3);
        }

        #endregion

        #region Grid↔World Round-Trip Tests

        [Test]
        public void GridToWorld_ThenWorldToGrid_ReturnsOriginalPosition()
        {
            var space = new ArenaSpace(new Vector3(10f, 0f, -5f));
            var originalGrid = new GridPosition(3, 2);

            var worldPos = space.GridToWorld(originalGrid);
            var resultGrid = space.WorldToGrid(worldPos);

            Assert.AreEqual(originalGrid, resultGrid);
        }

        [Test]
        public void GridToWorld_ThenWorldToGrid_WorksForAllValidPositions()
        {
            var space = new ArenaSpace(new Vector3(5f, 0f, 5f));

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    var originalGrid = new GridPosition(x, y);
                    var worldPos = space.GridToWorld(originalGrid);
                    var resultGrid = space.WorldToGrid(worldPos);

                    Assert.AreEqual(originalGrid, resultGrid, $"Failed for position ({x}, {y})");
                }
            }
        }

        #endregion

        [Test]
        public void GridToWorld_WithNegativeOrigin_WorksCorrectly()
        {
            var space = new ArenaSpace(new Vector3(-10f, 0f, -10f));

            var worldPos = space.GridToWorld(new GridPosition(2, 3));

            Assert.AreEqual(new Vector3(-8f, 0f, -7f), worldPos);
        }
    }
}
