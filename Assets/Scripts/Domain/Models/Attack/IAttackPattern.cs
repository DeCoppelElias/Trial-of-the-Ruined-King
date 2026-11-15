using System.Collections.Generic;

namespace Domain.Models
{
    public interface IAttackPattern
    {
        IEnumerable<GridPosition> GetAffectedArea();
        IEnumerable<GridPosition> GetActiveDangerPositions(AttackState state);
    }
}
