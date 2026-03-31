using System.Collections.Generic;

public class HammerAttack : Attack
{
    public GridPosition Middle { get; private set; }
    public HorizontalDirection Direction { get; private set; }

    public HammerAttack(GridPosition middle, HorizontalDirection direction, AttackTiming timing, IArena arena)
        : base(timing, new InstantAreaPattern(CalculateHammerPositions(middle, direction, arena), arena))
    {
        Middle = middle;
        Direction = direction;
    }

    private static IEnumerable<GridPosition> CalculateHammerPositions(GridPosition middle, HorizontalDirection direction, IArena arena)
    {
        var positions = new List<GridPosition> { middle };

        var topPosition = new GridPosition(middle.GridX, middle.GridY + 1);
        if (arena.IsInBounds(topPosition)) positions.Add(topPosition);
        
        var bottomPosition = new GridPosition(middle.GridX, middle.GridY - 1);
        if (arena.IsInBounds(bottomPosition)) positions.Add(bottomPosition);

        switch (direction)
        {
            case HorizontalDirection.LeftToRight:
                var rightPosition = new GridPosition(middle.GridX + 1, middle.GridY);
                if (arena.IsInBounds(rightPosition)) positions.Add(rightPosition);
                break;
            case HorizontalDirection.RightToLeft:
                var leftPosition = new GridPosition(middle.GridX - 1, middle.GridY);
                if (arena.IsInBounds(leftPosition)) positions.Add(leftPosition);
                break;
        }

        return positions;
    }
}