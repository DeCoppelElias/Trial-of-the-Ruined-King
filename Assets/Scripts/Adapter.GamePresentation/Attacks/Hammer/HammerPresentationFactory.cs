using Domain.Models;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Visual behavior for the hammer attack. During the telegraph phase, the hammer moves toward the board and grows in size.
/// </summary>
public class HammerPresentationFactory : IAttackPresentationFactory
{
    protected readonly ArenaSpace _arenaSpace;
    protected readonly Transform _parent;
    protected readonly TileVisualizer _tileVisualizer;
    protected readonly ArenaShaker _arenaShaker;
    protected readonly AudioPlayer _audioPlayer;

    protected readonly HammerVisualConfig _visualConfig;
    protected readonly HammerAudioConfig _audioConfig;

    public HammerPresentationFactory(
        HammerVisualConfig visualConfig,
        HammerAudioConfig audioConfig,
        ArenaSpace arenaSpace,
        Transform transform,
        TileVisualizer tileVisualizer,
        ArenaShaker arenaShaker,
        AudioPlayer audioPlayer)
    {
        _visualConfig = visualConfig;
        _audioConfig = audioConfig;
        _arenaSpace = arenaSpace;
        _parent = transform;
        _tileVisualizer = tileVisualizer;
        _arenaShaker = arenaShaker;
        _audioPlayer = audioPlayer;
    }

    public IAttackPresentation CreatePresentation(Attack attack)
    {
        if (attack is not HammerAttack)
            throw new System.ArgumentException("Invalid attack type for HammerPresentationFactory");

        return new HammerPresentation((HammerAttack) attack, _arenaSpace, _parent, _tileVisualizer, _arenaShaker, _audioPlayer, _visualConfig, _audioConfig);
    }
}