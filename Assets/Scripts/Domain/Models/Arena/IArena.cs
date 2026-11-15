using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public interface IArena
    {
        int Width { get; }
        int Height { get; }

        event Action OnArenaChanged;

        TileType GetTileType(GridPosition position);
        IEnumerable<(GridPosition position, TileType tileType)> GetAllTiles();
        bool IsInBounds(GridPosition position);
        GridPosition GetRandomEmptyGridPosition();
    }
}
