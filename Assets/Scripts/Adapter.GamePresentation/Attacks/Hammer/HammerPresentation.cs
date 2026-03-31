using Domain.Ports;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HammerPresentation : AttackPresentationBase
{
    private readonly IArena _arena;
    private readonly Transform _parent;
    private readonly ArenaShaker _arenaShaker;

    private readonly HammerVisualConfig _visualConfig;
    private readonly HammerAudioConfig _audioConfig;
    private readonly DomainUpdater _domainUpdator;

    private Renderer _renderer;
    private Animator _animator;
    private ParticleSystem _impactParticles;
    private HorizontalDirection _direction;
    private float _baseAnimatorSpeed = 1f;

    private Vector3 _telegraphStartPosition;
    private Vector3 _telegraphEndPosition;

    private float _telegraphDuration;

    private bool telegraphAnimationTriggered = false;
    private bool commitAnimationTriggered = false;
    private bool impactAnimationTriggered = false;

    private bool arenaShook = false;

    private string attackAnimationTrigger = "Attack";
    private string telegraphAnimationName = "Hammer Telegraph";
    private string commitAnimationName = "Hammer Commit";
    private string impactAnimationName = "Hammer Impact";

    public HammerPresentation(
        HammerAttack attack,
        IArena arena,
        Transform parent,
        TileVisualizer tileVisualizer,
        ArenaShaker arenaShaker,
        AudioPlayer audioPlayer,
        DomainUpdater domainUpdator,
        HammerVisualConfig visualConfig,
        HammerAudioConfig audioConfig) : base(attack, tileVisualizer, audioPlayer, audioConfig.CommitAudio)
    {
        _arena = arena;
        _parent = parent;
        _arenaShaker = arenaShaker;
        _domainUpdator = domainUpdator;

        _visualConfig = visualConfig;
        _audioConfig = audioConfig;

        _telegraphDuration = attack.Timing.GetDurationForStage(AttackStage.Telegraph);
        _direction = attack.Direction;

        CalculateTelegraphPositions(attack);
    }

    public override void OnStageChanged(AttackStage newStage)
    {
        switch (newStage)
        {
            case AttackStage.Telegraph:
                SpawnHammer();
                break;
            case AttackStage.Commit:
                break;
            case AttackStage.Impact:
                HandleImpactParticles();
                break;
            case AttackStage.Recovery:
                DisableCommitWarning();
                break;
        }
    }

    public override void UpdatePresentation(Attack attack)
    {
        if (_instance == null)
            return;

        if (_animator != null) _animator.speed = _baseAnimatorSpeed * _domainUpdator.CurrentTimeScale;

        float elapsed = attack.ElapsedTimeInStage;

        if (attack.CurrentStage == AttackStage.Telegraph)
        {
            UpdateTelegraphStage(elapsed);
        }
        else if (attack.CurrentStage == AttackStage.Commit)
        {
            UpdateCommitStage(attack, elapsed);
        }
        else if (attack.CurrentStage == AttackStage.Impact)
        {
            UpdateImpactStage();
        }
    }

    public override void Destroy()
    {
        if (_instance != null)
            Object.Destroy(_instance);
    }

    private void CalculateTelegraphPositions(HammerAttack attack)
    {
        Vector3 attackMiddlePosition = _arena.GridToWorld(attack.Middle);
        float directionMultiplier = attack.Direction == HorizontalDirection.RightToLeft ? 1f : -1f;

        _telegraphStartPosition = attackMiddlePosition + new Vector3(
            directionMultiplier * _visualConfig.TelegraphStartOffsetX,
            _visualConfig.TelegraphOffsetY,
            0f);

        _telegraphEndPosition = attackMiddlePosition + new Vector3(
            directionMultiplier * _visualConfig.TelegraphEndOffsetX,
            _visualConfig.TelegraphOffsetY,
            0f);
    }

    private void SpawnHammer()
    {
        if (_visualConfig.HammerPrefab == null)
            return;

        InstantiateHammer();
        CacheHammerComponents();
        InitializeHammerAppearance();
        InitializeCommitFlashIndicator();
        NotifyAttackSpawned();
    }

    private void InstantiateHammer()
    {
        Quaternion rotation = GetHammerRotation();
        _instance = Object.Instantiate(_visualConfig.HammerPrefab, _telegraphStartPosition, rotation, _parent);
        _instance.name = "HammerInstance";
        _instance.transform.localScale = _visualConfig.InitialScale;
    }

    private Quaternion GetHammerRotation()
    {
        float yRotation = _direction == HorizontalDirection.LeftToRight ? 0f : 180f;
        return Quaternion.Euler(0f, yRotation, 0f);
    }

    private void CacheHammerComponents()
    {
        _renderer = _instance.GetComponentInChildren<Renderer>();
        _animator = _instance.GetComponentInChildren<Animator>();
        _impactParticles = _instance.GetComponentInChildren<ParticleSystem>();
    }

    private void InitializeHammerAppearance()
    {
        if (_renderer != null && _renderer.material != null)
        {
            Color color = _renderer.material.color;
            color.a = 0f;
            _renderer.material.color = color;
        }
    }

    private void UpdateTelegraphStage(float elapsed)
    {
        FadeInHammer(elapsed);
        MoveHammerToStrikePosition(elapsed);
        GrowHammer(elapsed);

        if (!telegraphAnimationTriggered)
        {
            ChangeAnimatorSpeed(telegraphAnimationName, _attack.Timing.TelegraphDuration);
            _animator.SetTrigger(attackAnimationTrigger);
            telegraphAnimationTriggered = true;
        }

        float progress = elapsed / _telegraphDuration;
        if (!_commitWarningTriggered && progress > 0.9f)
            EnableCommitWarning();
    }

    private void FadeInHammer(float elapsed)
    {
        if (_renderer == null || _renderer.material == null)
            return;

        float fadeInDuration = _telegraphDuration * 0.1f;
        float fadeInProgress = Mathf.Clamp01(elapsed / fadeInDuration);
        Color color = _renderer.material.color;
        color.a = Mathf.Lerp(0f, 1f, fadeInProgress);
        _renderer.material.color = color;
    }

    private void MoveHammerToStrikePosition(float elapsed)
    {
        float movementEndTime = 0.6f * _telegraphDuration;
        if (elapsed < movementEndTime)
        {
            float t = elapsed / movementEndTime;
            _instance.transform.position = Vector3.Lerp(_telegraphStartPosition, _telegraphEndPosition, t);
        }
    }

    private void GrowHammer(float elapsed)
    {
        float growStartTime = 0.6f * _telegraphDuration;
        float growEndTime = 0.8f * _telegraphDuration;

        if (elapsed >= growStartTime && elapsed <= growEndTime)
        {
            float t = (elapsed - growStartTime) / (growEndTime - growStartTime);
            _instance.transform.localScale = Vector3.Lerp(_visualConfig.InitialScale, _visualConfig.FinalScale, t);
        }
        else if (elapsed > growEndTime)
        {
            _instance.transform.localScale = _visualConfig.FinalScale;
        }
    }

    private void UpdateCommitStage(Attack attack, float elapsed)
    {
        if (_animator == null)
            return;

        if (!commitAnimationTriggered)
        {
            ChangeAnimatorSpeed(commitAnimationName, _attack.Timing.CommitDuration);
            commitAnimationTriggered = true;
        }
    }

    private void ChangeAnimatorSpeed(string clipName, float stageDuration)
    {
        var clip = GetAnimationClip(clipName);

        float animationLength = clip.length;
        _baseAnimatorSpeed = animationLength / stageDuration;
        _animator.speed = _baseAnimatorSpeed * _domainUpdator.CurrentTimeScale;
    }

    private AnimationClip GetAnimationClip(string clipName)
    {
        return _animator.runtimeAnimatorController.animationClips.FirstOrDefault(c => c.name == clipName);
    }

    private void UpdateImpactStage()
    {
        if (!arenaShook)
        {
            ShakeArena();
            PlayImpactSound();
            arenaShook = true;
        }

        if (!impactAnimationTriggered)
        {
            ChangeAnimatorSpeed(impactAnimationName, _attack.Timing.ImpactDuration);
            impactAnimationTriggered = true;
        }
    }

    private void ShakeArena()
    {
        _arenaShaker.Shake(_visualConfig.ArenaShakeDuration, _visualConfig.ArenaShakeMagnitude);
    }

    private void PlayImpactSound()
    {
        _audioPlayer.Play(_audioConfig.HammerSlam);
    }

    private void HandleImpactParticles()
    {
        if (_impactParticles == null)
            return;

        ScaleImpactParticles();
        RestartParticles();
    }

    private void ScaleImpactParticles()
    {
        float scale = _instance.transform.localScale.x;

        var main = _impactParticles.main;
        main.startSize = main.startSize.constant * scale;
        main.startSpeed = main.startSpeed.constant * scale;

        var shape = _impactParticles.shape;
        shape.radius = shape.radius *= scale;
    }

    private void RestartParticles()
    {
        _impactParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _impactParticles.Play();
    }
}