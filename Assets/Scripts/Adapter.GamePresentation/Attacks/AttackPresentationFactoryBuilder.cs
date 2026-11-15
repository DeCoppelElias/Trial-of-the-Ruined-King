using System;
using UnityEngine;

/// <summary>
/// Concrete implementation that builds presentation factories based on attack definition types.
/// Maps each attack definition type to its corresponding factory.
/// </summary>
public class AttackPresentationFactoryBuilder : IAttackPresentationFactoryBuilder
{
    public IAttackPresentationFactory Build(
        IAttackDefinition definition,
        ArenaSpace arenaSpace,
        Transform parent,
        TileVisualizer tileVisualizer,
        AudioPlayer audioPlayer,
        ArenaShaker arenaShaker,
        Func<float> timeScale)
    {
        return definition switch
        {
            SpearRowAttackDefinition spearRow => new SpearRowPresentationFactory(
                spearRow.visualConfig,
                spearRow.audioConfig,
                arenaSpace,
                parent,
                tileVisualizer,
                audioPlayer),

            SpearColumnAttackDefinition spearColumn => new SpearColumnPresentationFactory(
                spearColumn.visualConfig,
                spearColumn.audioConfig,
                arenaSpace,
                parent,
                tileVisualizer,
                audioPlayer),

            HammerAttackDefinition hammer => new HammerPresentationFactory(
                hammer.visualConfig,
                hammer.audioConfig,
                arenaSpace,
                parent,
                tileVisualizer,
                arenaShaker,
                audioPlayer,
                timeScale),

            _ => throw new NotSupportedException($"No factory builder implementation for attack definition type: {definition.GetType().Name}")
        };
    }
}
