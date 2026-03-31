using System.Collections.Generic;

public interface IAttackPattern
{
    IEnumerable<GridPosition> GetAffectedArea();
    IEnumerable<GridPosition> GetActiveDangerPositions(AttackState state);
}
