using System;
using System.Collections.Generic;
using UnityEngine;

public class RectangleArena : IArena
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Vector3 WorldPosition { get; private set; }

    public event Action OnArenaChanged;

    private TileType[,] _tiles;

    public RectangleArena(int width, int height, Vector3 worldPosition)  
    {
        Width = width;
        Height = height;
        WorldPosition = worldPosition;
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

    public GridPosition WorldToGrid(Vector3 worldPosition)
    {
        Vector3 localPosition = worldPosition - WorldPosition;
        int gridX = Mathf.FloorToInt(localPosition.x);
        int gridY = Mathf.FloorToInt(localPosition.z);
        return new GridPosition(gridX, gridY);
    }

    public Vector3 GridToWorld(GridPosition gridPosition)
    {
        return WorldPosition + new Vector3(gridPosition.GridX, 0, gridPosition.GridY);
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

        int randomIndex = UnityEngine.Random.Range(0, emptyPositions.Count);
        return emptyPositions[randomIndex];
    }
}