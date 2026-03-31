using System.Collections.Generic;
using System.Linq;

public class InstantAreaPattern : IAttackPattern
{
    private readonly List<GridPosition> _affectedPositions;

    public InstantAreaPattern(IEnumerable<GridPosition> positions, IArena arena)
    {
        _affectedPositions = new List<GridPosition>();
        foreach (var position in positions)
        {
            if (arena.IsInBounds(position))
            {
                _affectedPositions.Add(position);
            }
        }
    }

    public InstantAreaPattern(GridPosition center, IArena arena)
    {
        _affectedPositions = new List<GridPosition>();
        if (arena.IsInBounds(center))
        {
            _affectedPositions.Add(center);
        }
    }

    public IEnumerable<GridPosition> GetAffectedArea()
    {
        return _affectedPositions;
    }

    public IEnumerable<GridPosition> GetActiveDangerPositions(AttackState state)
    {
        return state.Stage == AttackStage.Impact 
            ? _affectedPositions 
            : Enumerable.Empty<GridPosition>();
    }
}
