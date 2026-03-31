using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpearColumnAttackFactory : IAttackFactory
{
    private readonly IArena _arena;

    public SpearColumnAttackFactory(IArena arena)
    {
        _arena = arena;
    }

    public Attack CreateRandomAttack(AttackDefinitionBase definition, float attackTimingMultiplier, IReadOnlyCollection<GridPosition> occupiedTiles, float minFreePercentage)
    {
        var availableColumns = GetAvailableColumns(occupiedTiles);
        if (availableColumns.Count == 0)
            return null;

        int randomColumn = availableColumns[Random.Range(0, availableColumns.Count)];
        VerticalDirection direction = Random.value < 0.5f
            ? VerticalDirection.BottomToTop
            : VerticalDirection.TopToBottom;

        var timing = definition.attackTiming.Multiply(attackTimingMultiplier);
        var attack = new SpearColumnAttack(randomColumn, direction, timing, _arena);

        if (!MeetsFreePercentageRequirement(attack, occupiedTiles, minFreePercentage))
            return null;

        return attack;
    }

    public Attack CreateSpecificAttack(AttackStepParametersBase parameters, AttackTiming timing)
    {
        if (parameters is SpearColumnParameters spearColumnParams)
        {
            return new SpearColumnAttack(spearColumnParams.column, spearColumnParams.columnDirection, timing, _arena);
        }

        Debug.LogError($"SpearColumnAttackFactory received incorrect parameter type: {parameters?.GetType().Name}");
        return null;
    }

    private List<int> GetAvailableColumns(IReadOnlyCollection<GridPosition> occupiedTiles)
    {
        var availableColumns = new List<int>();
        for (int column = 0; column < _arena.Width; column++)
        {
            if (IsColumnFree(column, occupiedTiles))
                availableColumns.Add(column);
        }
        return availableColumns;
    }

    private bool IsColumnFree(int column, IReadOnlyCollection<GridPosition> occupiedTiles)
    {
        for (int y = 0; y < _arena.Height; y++)
        {
            if (occupiedTiles.Contains(new GridPosition(column, y)))
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
