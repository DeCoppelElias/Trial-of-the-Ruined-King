using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Visual behavior for SpearColumnAttack - creates a projectile that flies vertically across the arena.
/// </summary>
public class SpearColumnPresentationFactory : SpearPresentationBaseFactory
{
    public SpearColumnPresentationFactory(
        SpearVisualConfig visualConfig,
        SpearAudioConfig audioConfig,
        IArena arena,
        Transform parent,
        TileVisualizer tileVisualizer,
        AudioPlayer audioPlayer)
        : base(visualConfig, audioConfig, arena, parent, tileVisualizer, audioPlayer)
    {
    }

    protected override SpearPresentationBase CreateSpearPresentation(
        Attack attack)
    {
        return new SpearColumnPresentation(
            attack,
            _arena,
            _parent,
            _tileVisualizer,
            _audioPlayer,
            _visualConfig,
            _audioConfig);
    }
}