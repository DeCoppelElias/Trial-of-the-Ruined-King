using Domain.Models;
using UnityEngine;

/// <summary>
/// Maps arena tiles to positions in the scene. The domain arena knows only tiles;
/// where the arena sits in the world is a presentation concern.
/// </summary>
public class ArenaSpace
{
    public Vector3 Origin { get; }

    public ArenaSpace(Vector3 origin)
    {
        Origin = origin;
    }

    public Vector3 GridToWorld(GridPosition gridPosition)
    {
        return Origin + new Vector3(gridPosition.GridX, 0, gridPosition.GridY);
    }

    public GridPosition WorldToGrid(Vector3 worldPosition)
    {
        Vector3 localPosition = worldPosition - Origin;
        return new GridPosition(Mathf.FloorToInt(localPosition.x), Mathf.FloorToInt(localPosition.z));
    }
}
