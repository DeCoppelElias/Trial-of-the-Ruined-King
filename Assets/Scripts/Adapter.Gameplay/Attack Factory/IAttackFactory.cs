using System.Collections.Generic;
using Domain.Ports;

public interface IAttackFactory
{
    Attack CreateRandomAttack(AttackDefinitionBase definition, float attackTimingMultiplier, IReadOnlyCollection<GridPosition> occupiedTiles, float minFreePercentage);
    Attack CreateSpecificAttack(AttackStepParametersBase parameters, AttackTiming timing);
}