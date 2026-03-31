using Domain.Ports;
using UnityEngine;

/// <summary>
/// Responsible for building IAttackPresentationFactory instances from attack definitions
/// and runtime dependencies. Separates factory creation logic from ScriptableObject data.
/// </summary>
public interface IAttackPresentationFactoryBuilder
{
    /// <summary>
    /// Builds a factory for creating attack presentations.
    /// </summary>
    /// <param name="definition">The attack definition containing configuration data</param>
    /// <param name="arena">The game arena</param>
    /// <param name="parent">Parent transform for instantiated visuals</param>
    /// <param name="tileVisualizer">Visualizer for tile danger indicators</param>
    /// <param name="audioPlayer">Audio player for sound effects</param>
    /// <param name="arenaShaker">Shaker for arena shake effects</param>
    /// <param name="timeService">Time service for time-scaled animations</param>
    /// <returns>A factory that can create attack presentations</returns>
    IAttackPresentationFactory Build(
        IAttackDefinition definition,
        IArena arena,
        Transform parent,
        TileVisualizer tileVisualizer,
        AudioPlayer audioPlayer,
        ArenaShaker arenaShaker,
        DomainUpdater domainUpdator);
}
