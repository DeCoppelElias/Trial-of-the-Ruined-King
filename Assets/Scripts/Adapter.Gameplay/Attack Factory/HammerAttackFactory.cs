using System.Collections.Generic;
using System.Linq;
using Domain.Ports;
using UnityEngine;

public class HammerAttackFactory : IAttackFactory
{
    private readonly IArena _arena;

    public HammerAttackFactory(IArena arena)
    {
        _arena = arena;
    }

    public Attack CreateRandomAttack(AttackDefinitionBase definition, float attackTimingMultiplier, IReadOnlyCollection<GridPosition> occupiedTiles, float minFreePercentage)
    {
        var availablePositions = GetAvailablePositions(occupiedTiles, definition, attackTimingMultiplier, minFreePercentage);
        if (availablePositions.Count == 0)
            return null;

        var selectedPosition = availablePositions[Random.Range(0, availablePositions.Count)];
        HorizontalDirection direction = selectedPosition.GridX == 0
            ? HorizontalDirection.LeftToRight
            : HorizontalDirection.RightToLeft;

        var timing = definition.attackTiming.Multiply(attackTimingMultiplier);
        return new HammerAttack(selectedPosition, direction, timing, _arena);
    }

    public Attack CreateSpecificAttack(AttackStepParametersBase parameters, AttackTiming timing)
    {
        if (parameters is HammerParameters hammerParams)
        {
            if (hammerParams.hammerPosition == null)
                return null;

            return new HammerAttack(hammerParams.hammerPosition, hammerParams.hammerDirection, timing, _arena);
        }

        Debug.LogError($"HammerAttackFactory received incorrect parameter type: {parameters?.GetType().Name}");
        return null;
    }

    private List<GridPosition> GetAvailablePositions(IReadOnlyCollection<GridPosition> occupiedTiles, AttackDefinitionBase definition, float attackTimingMultiplier, float minFreePercentage)
    {
        var availablePositions = new List<GridPosition>();
        var timing = definition.attackTiming.Multiply(attackTimingMultiplier);

        for (int y = 0; y < _arena.Height; y++)
        {
            foreach (var direction in new[] { HorizontalDirection.LeftToRight, HorizontalDirection.RightToLeft })
            {
                var x = direction == HorizontalDirection.LeftToRight ? 0 : _arena.Width - 1;
                var position = new GridPosition(x, y);
                var testAttack = new HammerAttack(position, direction, timing, _arena);

                if (MeetsFreePercentageRequirement(testAttack, occupiedTiles, minFreePercentage))
                    availablePositions.Add(position);
            }
        }

        return availablePositions;
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
