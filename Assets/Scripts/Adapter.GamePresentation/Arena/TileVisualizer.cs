using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager class that tracks all tile GameObjects in the arena and provides
/// access to enable/disable danger indicators on specific tiles.
/// Used by AttackVisualizer to show danger warnings on affected tiles.
/// </summary>
public class TileVisualizer : MonoBehaviour
{
    [SerializeField] private Transform _tileParent;

    private Dictionary<GridPosition, Tile> _tiles;
    private Dictionary<GridPosition, int> _dangerReferenceCounts;
    private IArena _arena;

    public void Initialize(IArena arena, ArenaPresenter arenaPresenter)
    {
        _arena = arena;
        _tiles = new Dictionary<GridPosition, Tile>();
        _dangerReferenceCounts = new Dictionary<GridPosition, int>();

        arenaPresenter.OnArenaVisualized += OnArenaVisualized;
    }

    private void OnArenaVisualized()
    {
        // Find all Tile components under the TileParent
        Tile[] tiles = _tileParent.GetComponentsInChildren<Tile>();
        foreach (Tile tile in tiles)
        {
            // Convert tile's world position to grid position
            Vector3 tileWorldPosition = tile.transform.position;
            GridPosition gridPosition = _arena.WorldToGrid(tileWorldPosition);

            // Verify the position is valid and within arena bounds
            if (_arena.IsInBounds(gridPosition))
            {
                if (_tiles.ContainsKey(gridPosition))
                {
                    Debug.LogWarning($"TileVisualizer: Multiple tiles found at grid position {gridPosition}. Using the latest one.");
                }
                _tiles[gridPosition] = tile;
                _dangerReferenceCounts[gridPosition] = 0;
            }
            else
            {
                Debug.LogWarning($"TileVisualizer: Tile at world position {tileWorldPosition} maps to out-of-bounds grid position {gridPosition}");
            }
        }

        Debug.Log($"TileVisualizer initialized with {_tiles.Count} tiles.");
    }

    /// <summary>
    /// Enables the danger indicator on a specific tile.
    /// Uses reference counting to handle overlapping dangers from multiple attacks.
    /// </summary>
    public void EnableDangerIndicator(GridPosition position)
    {
        if (!_tiles.TryGetValue(position, out Tile tile)) return;

        // Increment reference count
        if (!_dangerReferenceCounts.ContainsKey(position))
        {
            _dangerReferenceCounts[position] = 0;
        }
        _dangerReferenceCounts[position]++;

        // Show indicator (Tile will handle if already active)
        tile.ShowDangerIndicator();
    }

    /// <summary>
    /// Disables the danger indicator on a specific tile.
    /// Only hides the indicator when all danger sources have been cleared.
    /// </summary>
    public void DisableDangerIndicator(GridPosition position)
    {
        if (!_tiles.TryGetValue(position, out Tile tile)) return;

        // Decrement reference count
        if (!_dangerReferenceCounts.ContainsKey(position))
        {
            Debug.LogWarning($"TileVisualizer: Attempted to disable danger indicator at {position} but no reference count exists.");
            return;
        }

        _dangerReferenceCounts[position]--;

        // Prevent negative counts
        if (_dangerReferenceCounts[position] < 0)
        {
            Debug.LogWarning($"TileVisualizer: Danger reference count at {position} went negative. Resetting to 0.");
            _dangerReferenceCounts[position] = 0;
        }

        // Only hide when all danger sources are cleared
        if (_dangerReferenceCounts[position] == 0)
        {
            tile.HideDangerIndicator();
        }
    }

    /// <summary>
    /// Enables danger indicators on multiple tiles.
    /// </summary>
    public void EnableDangerIndicators(IEnumerable<GridPosition> positions)
    {
        foreach (GridPosition position in positions)
        {
            EnableDangerIndicator(position);
        }
    }

    /// <summary>
    /// Disables danger indicators on multiple tiles.
    /// </summary>
    public void DisableDangerIndicators(IEnumerable<GridPosition> positions)
    {
        foreach (GridPosition position in positions)
        {
            DisableDangerIndicator(position);
        }
    }

    /// <summary>
    /// Disables all danger indicators in the arena and resets reference counts.
    /// </summary>
    public void DisableAllDangerIndicators()
    {
        foreach (Tile tile in _tiles.Values)
        {
            tile.HideDangerIndicator();
        }
        _dangerReferenceCounts.Clear();
    }

    /// <summary>
    /// Gets the Tile component at a specific grid position.
    /// Returns null if no tile exists at that position.
    /// </summary>
    public Tile GetTile(GridPosition position)
    {
        _tiles.TryGetValue(position, out Tile tile);
        return tile;
    }

    /// <summary>
    /// Checks if a danger indicator is active at a specific position.
    /// </summary>
    public bool IsDangerIndicatorActive(GridPosition position)
    {
        if (_tiles.TryGetValue(position, out Tile tile))
        {
            return tile.IsDangerIndicatorActive();
        }
        return false;
    }

    /// <summary>
    /// Gets the current danger reference count for a tile (for debugging).
    /// </summary>
    public int GetDangerReferenceCount(GridPosition position)
    {
        return _dangerReferenceCounts.TryGetValue(position, out int count) ? count : 0;
    }
}
