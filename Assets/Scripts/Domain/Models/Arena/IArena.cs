using System;
using System.Collections.Generic;
using UnityEngine;

public interface IArena
{
    int Width { get; }
    int Height { get; }
    Vector3 WorldPosition { get; }
    
    event Action OnArenaChanged;
    
    TileType GetTileType(GridPosition position);
    IEnumerable<(GridPosition position, TileType tileType)> GetAllTiles();
    bool IsInBounds(GridPosition position);
    GridPosition GetRandomEmptyGridPosition();

    GridPosition WorldToGrid(Vector3 worldPosition);
    Vector3 GridToWorld(GridPosition gridPosition);
}