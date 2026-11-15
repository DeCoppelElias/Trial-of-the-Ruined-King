using Domain.Models;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Visual behavior for SpearRowAttack - creates a projectile that flies across the arena.
/// </summary>
public class SpearRowPresentationFactory : SpearPresentationBaseFactory
{
    public SpearRowPresentationFactory(
        SpearVisualConfig visualConfig,
        SpearAudioConfig audioConfig,
        ArenaSpace arenaSpace,
        Transform parent,
        TileVisualizer tileVisualizer,
        AudioPlayer audioPlayer)
        : base(visualConfig, audioConfig, arenaSpace, parent, tileVisualizer, audioPlayer)
    {
    }

    protected override SpearPresentationBase CreateSpearPresentation(Attack attack)
    {
        return new SpearRowPresentation(
            attack,
            _arenaSpace,
            _parent,
            _tileVisualizer,
            _audioPlayer,
            _visualConfig,
            _audioConfig);
    }
}