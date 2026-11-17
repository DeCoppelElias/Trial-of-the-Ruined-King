using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class RectangleArena : IArena
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public event Action OnArenaChanged;

        private TileType[,] _tiles;
        private readonly RandomSource _random;

        public RectangleArena(int width, int height, RandomSource random)
        {
            Width = width;
            Height = height;
            _random = random ?? throw new ArgumentNullException(nameof(random));
            InitializeTiles();
        }

        private void InitializeTiles()
        {
            _tiles = new TileType[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    _tiles[x, y] = TileType.Floor;
                }
            }
        }

        public TileType GetTileType(GridPosition position)
        {
            if (!IsInBounds(position))
            {
                return TileType.Empty;
            }

            return _tiles[position.GridX, position.GridY];
        }

        public IEnumerable<(GridPosition position, TileType tileType)> GetAllTiles()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    GridPosition position = new GridPosition(x, y);
                    yield return (position, _tiles[x, y]);
                }
            }
        }

        public bool IsInBounds(GridPosition position)
        {
            return position.GridX >= 0 && position.GridX < Width && 
                   position.GridY >= 0 && position.GridY < Height;
        }

        public GridPosition GetRandomEmptyGridPosition()
        {
            List<GridPosition> emptyPositions = new List<GridPosition>();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    GridPosition position = new GridPosition(x, y);
                    if (GetTileType(position) == TileType.Floor)
                    {
                        emptyPositions.Add(position);
                    }
                }
            }

            if (emptyPositions.Count == 0)
            {
                return null;
            }

            return _random.Pick(emptyPositions);
        }
    }
}
