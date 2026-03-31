using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for spear visual behaviors that creates projectiles flying across the arena.
/// </summary>
public abstract class SpearPresentationBaseFactory : IAttackPresentationFactory
{
    protected readonly SpearVisualConfig _visualConfig;
    protected readonly SpearAudioConfig _audioConfig;
    protected readonly IArena _arena;
    protected readonly Transform _parent;
    protected readonly TileVisualizer _tileVisualizer;
    protected readonly AudioPlayer _audioPlayer;

    protected SpearPresentationBaseFactory(
        SpearVisualConfig visualConfig,
        SpearAudioConfig audioConfig,
        IArena arena,
        Transform parent,
        TileVisualizer tileVisualizer,
        AudioPlayer audioPlayer)
    {
        _visualConfig = visualConfig;
        _audioConfig = audioConfig;
        _arena = arena;
        _parent = parent;
        _tileVisualizer = tileVisualizer;
        _audioPlayer = audioPlayer;
    }

    public IAttackPresentation CreatePresentation(Attack attack)
    {
        return CreateSpearPresentation(attack);
    }

    protected abstract SpearPresentationBase CreateSpearPresentation(Attack attack);
}