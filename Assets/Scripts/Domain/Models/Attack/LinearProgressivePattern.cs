using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LinearProgressivePattern : IProgressivePattern
{
    private readonly int _rowOrColumn;
    private readonly bool _isRow;
    private readonly HorizontalDirection _horizontalDirection;
    private readonly VerticalDirection _verticalDirection;
    private readonly IArena _arena;
    private readonly List<GridPosition> _affectedPositions;

    public LinearProgressivePattern(int rowOrColumn, HorizontalDirection direction, IArena arena)
    {
        _rowOrColumn = rowOrColumn;
        _isRow = true;
        _horizontalDirection = direction;
        _arena = arena;
        _affectedPositions = new List<GridPosition>();

        for (int x = 0; x < arena.Width; x++)
        {
            var pos = new GridPosition(x, rowOrColumn);
            if (arena.IsInBounds(pos))
            {
                _affectedPositions.Add(pos);
            }
        }
    }

    public LinearProgressivePattern(int rowOrColumn, VerticalDirection direction, IArena arena)
    {
        _rowOrColumn = rowOrColumn;
        _isRow = false;
        _verticalDirection = direction;
        _arena = arena;
        _affectedPositions = new List<GridPosition>();

        for (int y = 0; y < arena.Height; y++)
        {
            var pos = new GridPosition(rowOrColumn, y);
            if (arena.IsInBounds(pos))
            {
                _affectedPositions.Add(pos);
            }
        }
    }

    public IEnumerable<GridPosition> GetAffectedArea()
    {
        return _affectedPositions;
    }

    public float GetProgress(AttackState state)
    {
        if (state.Stage == AttackStage.Commit)
            return state.ProgressInStage;
        
        if (state.Stage == AttackStage.Impact)
            return 1.0f;
        
        return 0f;
    }

    public IEnumerable<GridPosition> GetActiveDangerPositions(AttackState state)
    {
        if (state.Stage != AttackStage.Commit && state.Stage != AttackStage.Impact)
            return Enumerable.Empty<GridPosition>();

        float progress = GetProgress(state);
        int tileCount = _isRow ? _arena.Width : _arena.Height;
        int currentTileIndex = Mathf.FloorToInt(progress * tileCount);
        currentTileIndex = Mathf.Clamp(currentTileIndex, 0, tileCount - 1);

        if (_isRow)
        {
            int xPosition = _horizontalDirection == HorizontalDirection.LeftToRight
                ? currentTileIndex
                : (tileCount - 1) - currentTileIndex;
            
            return new[] { new GridPosition(xPosition, _rowOrColumn) };
        }
        else
        {
            int yPosition = _verticalDirection == VerticalDirection.BottomToTop
                ? currentTileIndex
                : (tileCount - 1) - currentTileIndex;
            
            return new[] { new GridPosition(_rowOrColumn, yPosition) };
        }
    }
}
