using Domain.Models;
using System.Collections.Generic;

public interface IAttackFactory
{
    Attack CreateRandomAttack(AttackDefinitionBase definition, float attackTimingMultiplier, IReadOnlyCollection<GridPosition> occupiedTiles, float minFreePercentage);
    Attack CreateSpecificAttack(AttackStepParametersBase parameters, AttackTiming timing);
}