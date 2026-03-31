
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackPresentationBase : IAttackPresentation
{
    public GameObject _instance;

    protected readonly List<GridPosition> _affectedPositions = new();
    protected readonly TileVisualizer _tileVisualizer;
    protected readonly AudioPlayer _audioPlayer;
    protected readonly Attack _attack;

    protected FlashIndicator _commitIndicator;
    protected AudioEvent _commitAudioEvent;
    protected bool _commitWarningTriggered;

    public event Action<GameObject> OnAttackSpawned;

    protected AttackPresentationBase(Attack attack, TileVisualizer tileVisualizer, AudioPlayer audioPlayer, AudioEvent commitAudioEvent)
    {
        _attack = attack;
        _affectedPositions.AddRange(attack.Pattern.GetAffectedArea());
        _tileVisualizer = tileVisualizer;
        _audioPlayer = audioPlayer;
        _commitAudioEvent = commitAudioEvent;
        _commitWarningTriggered = false;
    }

    public abstract void Destroy();

    public abstract void OnStageChanged(AttackStage newStage);

    public abstract void UpdatePresentation(Attack attack);

    protected void InitializeCommitFlashIndicator()
    {
        if (_instance == null) return;
        Transform commitTransform = _instance.transform.Find("Commit Indicator");

        if (commitTransform != null)
        {
            _commitIndicator = commitTransform.GetComponent<FlashIndicator>();
            if (_commitIndicator != null)
            {
                commitTransform.gameObject.SetActive(false);
            }
        }
    }

    protected virtual void EnableCommitWarning()
    {
        if (_commitWarningTriggered) return;

        if (_commitIndicator != null) _commitIndicator.Flash();
        if (_tileVisualizer != null) _tileVisualizer.EnableDangerIndicators(_affectedPositions);
        if (_audioPlayer != null) _audioPlayer.Play(_commitAudioEvent);
        _commitWarningTriggered = true;
    }

    protected virtual void DisableCommitWarning()
    {
        if (_tileVisualizer != null) _tileVisualizer.DisableDangerIndicators(_affectedPositions);
    }

    protected void NotifyAttackSpawned()
    {
        OnAttackSpawned?.Invoke(_instance);
    }
}