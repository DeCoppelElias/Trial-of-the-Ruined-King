using System.Collections.Generic;
using System.Linq;
using Domain.Ports;
using UnityEngine;

public class SpearRowAttackFactory : IAttackFactory
{
    private readonly IArena _arena;

    public SpearRowAttackFactory(IArena arena)
    {
        _arena = arena;
    }

    public Attack CreateRandomAttack(AttackDefinitionBase definition, float attackTimingMultiplier, IReadOnlyCollection<GridPosition> occupiedTiles, float minFreePercentage)
    {
        var availableRows = GetAvailableRows(occupiedTiles);
        if (availableRows.Count == 0)
            return null;

        int randomRow = availableRows[Random.Range(0, availableRows.Count)];
        HorizontalDirection direction = Random.value < 0.5f
            ? HorizontalDirection.LeftToRight
            : HorizontalDirection.RightToLeft;

        var timing = definition.attackTiming.Multiply(attackTimingMultiplier);
        var attack = new SpearRowAttack(randomRow, direction, timing, _arena);

        if (!MeetsFreePercentageRequirement(attack, occupiedTiles, minFreePercentage))
            return null;

        return attack;
    }

    public Attack CreateSpecificAttack(AttackStepParametersBase parameters, AttackTiming timing)
    {
        if (parameters is SpearRowParameters spearRowParams)
        {
            return new SpearRowAttack(spearRowParams.row, spearRowParams.rowDirection, timing, _arena);
        }

        Debug.LogError($"SpearRowAttackFactory received incorrect parameter type: {parameters?.GetType().Name}");
        return null;
    }

    private List<int> GetAvailableRows(IReadOnlyCollection<GridPosition> occupiedTiles)
    {
        var availableRows = new List<int>();
        for (int row = 0; row < _arena.Height; row++)
        {
            if (IsRowFree(row, occupiedTiles))
                availableRows.Add(row);
        }
        return availableRows;
    }

    private bool IsRowFree(int row, IReadOnlyCollection<GridPosition> occupiedTiles)
    {
        for (int x = 0; x < _arena.Width; x++)
        {
            if (occupiedTiles.Contains(new GridPosition(x, row)))
                return false;
        }
        return true;
    }

    private bool MeetsFreePercentageRequirement(Attack attack, IReadOnlyCollection<GridPosition> occupiedTiles, float minFreePercentage)
    {
        var affectedPositions = attack.Pattern.GetAffectedArea().ToList();
        if (affectedPositions.Count == 0)
            return false;

        int freeCount = affectedPositions.Count(pos => !occupiedTiles.Contains(pos));
        float freePercentage = (float)freeCount / affectedPositions.Count;

        return freePercentage >= minFreePercentage;
    }
}
