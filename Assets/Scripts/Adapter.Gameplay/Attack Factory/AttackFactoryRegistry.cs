using System;
using System.Collections.Generic;

public class AttackFactoryRegistry : IAttackFactory
{
    private readonly Dictionary<Type, IAttackFactory> _factories = new Dictionary<Type, IAttackFactory>();

    public void Register(Type attackType, IAttackFactory factory)
    {
        _factories[attackType] = factory;
    }

    public Attack CreateRandomAttack(AttackDefinitionBase definition, float attackTimingMultiplier, IReadOnlyCollection<GridPosition> occupiedTiles, float minFreePercentage)
    {
        if (definition == null)
            return null;

        if (_factories.TryGetValue(definition.AttackType, out var factory))
        {
            return factory.CreateRandomAttack(definition, attackTimingMultiplier, occupiedTiles, minFreePercentage);
        }

        return null;
    }

    public Attack CreateSpecificAttack(AttackStepParametersBase parameters, AttackTiming timing)
    {
        if (parameters == null)
            return null;

        // Determine which factory to use based on parameter type
        Type attackType = GetAttackTypeFromParameters(parameters);
        if (attackType == null)
            return null;

        if (_factories.TryGetValue(attackType, out var factory))
        {
            return factory.CreateSpecificAttack(parameters, timing);
        }

        return null;
    }

    public Attack CreateSpecificAttackByType(AttackType attackType, AttackStepParametersBase parameters, AttackTiming timing)
    {
        var typeMapping = GetTypeMapping(attackType);
        if (typeMapping == null)
            return null;

        if (_factories.TryGetValue(typeMapping, out var factory))
        {
            return factory.CreateSpecificAttack(parameters, timing);
        }

        return null;
    }

    private Type GetAttackTypeFromParameters(AttackStepParametersBase parameters)
    {
        return parameters switch
        {
            SpearRowParameters => typeof(SpearRowAttack),
            SpearColumnParameters => typeof(SpearColumnAttack),
            HammerParameters => typeof(HammerAttack),
            _ => null
        };
    }

    private Type GetTypeMapping(AttackType attackType)
    {
        return attackType switch
        {
            AttackType.SpearRow => typeof(SpearRowAttack),
            AttackType.SpearColumn => typeof(SpearColumnAttack),
            AttackType.Hammer => typeof(HammerAttack),
            _ => null
        };
    }
}
