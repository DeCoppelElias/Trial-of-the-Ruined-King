using Codice.CM.Common;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpearPresentationBase : AttackPresentationBase
{
    private readonly IArena _arena;
    private readonly Transform _parent;

    private readonly SpearVisualConfig _visualConfig;
    private readonly SpearAudioConfig _audioConfig;

    private readonly List<GameObject> _telegraphIndicators;
    private Vector3 _spearStartPosition;
    private Vector3 _spearEndPosition;
    private Vector3 _spearChargeBackPosition;

    private float _telegraphDuration;
    private TrailRenderer _trailRenderer;
    private Renderer _renderer;

    private FlashIndicator _warningIndicator;

    private const float ChargeBackDistance = 0.3f;

    protected SpearPresentationBase(
        Attack attack,
        IArena arena,
        Transform parent,
        TileVisualizer tileVisualizer,
        AudioPlayer audioPlayer,
        SpearVisualConfig visualConfig,
        SpearAudioConfig audioConfig) : base(attack, tileVisualizer, audioPlayer, audioConfig.CommitAudioEvent)
    {
        _arena = arena;
        _parent = parent;
        _visualConfig = visualConfig;
        _audioConfig = audioConfig;

        _telegraphIndicators = new List<GameObject>();
        _telegraphDuration = attack.Timing.GetDurationForStage(AttackStage.Telegraph);
    }

    protected abstract void SortPositions(List<GridPosition> positions);
    protected abstract void DetermineStartAndEndPositions(List<GridPosition> sortedPositions, out GridPosition startPos, out GridPosition endPos);
    protected abstract Quaternion CalculateSpearRotation(Vector3 direction);
    protected abstract string GetSpearInstanceName();

    protected void CalculateSpearTrajectory(Attack attack)
    {
        List<GridPosition> positions = new List<GridPosition>(
            attack.Pattern.GetAffectedArea());

        if (positions.Count == 0) return;

        SortPositions(positions);

        GridPosition startPos;
        GridPosition endPos;
        DetermineStartAndEndPositions(positions, out startPos, out endPos);

        Vector3 startWorld = _arena.GridToWorld(startPos);
        Vector3 endWorld = _arena.GridToWorld(endPos);
        Vector3 direction = (endWorld - startWorld).normalized;

        float extraDistance = 2f;
        _spearStartPosition = startWorld - direction * extraDistance;
        _spearStartPosition.y = _visualConfig.SpearChargeY;

        _spearChargeBackPosition = _spearStartPosition - direction * ChargeBackDistance;
        _spearChargeBackPosition.y = _visualConfig.SpearChargeY;

        _spearEndPosition = endWorld + direction * extraDistance;
        _spearEndPosition.y = _visualConfig.SpearChargeY;
    }

    public override void OnStageChanged(AttackStage newStage)
    {
        switch (newStage)
        {
            case AttackStage.Telegraph:
                SpawnSpear();
                break;

            case AttackStage.Commit:
                ClearTelegraphIndicators();
                _audioPlayer.Play(_audioConfig.SpearWhooshAudioEvent);
                break;

            case AttackStage.Impact:
                break;

            case AttackStage.Recovery:
                DisableCommitWarning();
                break;
        }
    }

    public override void UpdatePresentation(Attack attack)
    {
        if (_instance == null) return;

        if (attack.CurrentStage == AttackStage.Telegraph)
        {
            HandleTelegraph(attack);
        }
        else if (attack.CurrentStage == AttackStage.Commit)
        {
            HandleCommit(attack);
        }
    }

    private void HandleTelegraph(Attack attack)
    {
        float elapsed = attack.ElapsedTimeInStage;

        float riseEndTime = _telegraphDuration * 0.4f;
        if (elapsed <= riseEndTime)
        {
            float riseProgress = elapsed / riseEndTime;
            float currentY = Mathf.Lerp(_visualConfig.SpearGroundY, _visualConfig.SpearChargeY, riseProgress);

            Vector3 position = _instance.transform.position;
            position.y = currentY;
            _instance.transform.position = position;
        }
        else if (elapsed <= _telegraphDuration * 0.6f)
        {
            float chargeStartTime = _telegraphDuration * 0.4f;
            float chargeEndTime = _telegraphDuration * 0.6f;
            float chargeProgress = (elapsed - chargeStartTime) / (chargeEndTime - chargeStartTime);

            Vector3 currentPosition = Vector3.Lerp(
                _spearStartPosition,
                _spearChargeBackPosition,
                chargeProgress);

            _instance.transform.position = currentPosition;
        }
        else
        {
            _instance.transform.position = _spearChargeBackPosition;
        }

        float commitStartTime = _telegraphDuration * 0.75f;
        if (elapsed >= commitStartTime && !_commitWarningTriggered) EnableCommitWarning();

        // Fade in spear during the first 10% of Telegraph phase
        if (_renderer != null && _renderer.material != null)
        {
            float fadeInDuration = _telegraphDuration * 0.1f;
            float fadeInProgress = Mathf.Clamp01(elapsed / fadeInDuration);
            Color color = _renderer.material.color;
            color.a = Mathf.Lerp(0f, 1f, fadeInProgress);
            _renderer.material.color = color;
        }
    }

    protected override void EnableCommitWarning()
    {
        base.EnableCommitWarning();
        if (_trailRenderer != null)
        {
            _trailRenderer.enabled = true;
        }
    }

    protected override void DisableCommitWarning()
    {
        base.DisableCommitWarning();
        if (_trailRenderer != null)
        {
            _trailRenderer.enabled = false;
        }
    }

    private void HandleCommit(Attack attack)
    {
        float movementProgress;
        
        if (attack.Pattern is IProgressivePattern progressive)
        {
            var state = attack.GetCurrentState();
            movementProgress = progressive.GetProgress(state);
        }
        else
        {
            float commitDuration = attack.Timing.GetDurationForStage(AttackStage.Commit);
            movementProgress = commitDuration > 0 ? attack.ElapsedTimeInStage / commitDuration : 1f;
        }

        _instance.transform.position = Vector3.Lerp(
            _spearChargeBackPosition,
            _spearEndPosition,
            movementProgress);

        MovementFadeOut(movementProgress);
    }

    private void MovementFadeOut(float movementProgress)
    {
        if (_renderer != null && _renderer.material != null)
        {
            // Fade starts at 90% progress, ends at 100%
            float fadeStart = 0.9f;
            float fadeEnd = 1.0f;
            float fadeProgress = Mathf.InverseLerp(fadeStart, fadeEnd, movementProgress);

            Color color = _renderer.material.color;
            color.a = Mathf.Lerp(1f, 0f, fadeProgress);
            _renderer.material.color = color;

            // Fade out trail renderer by offsetting alpha keys
            if (_trailRenderer != null)
            {
                Gradient originalGradient = _trailRenderer.colorGradient;
                GradientColorKey[] colorKeys = originalGradient.colorKeys;
                GradientAlphaKey[] originalAlphaKeys = originalGradient.alphaKeys;

                // Offset all alpha key times by fadeProgress, clamp to [0,1]
                GradientAlphaKey[] newAlphaKeys = new GradientAlphaKey[originalAlphaKeys.Length];
                for (int i = 0; i < originalAlphaKeys.Length; i++)
                {
                    float newTime = Mathf.Clamp01(originalAlphaKeys[i].time + fadeProgress * (1f - originalAlphaKeys[i].time));
                    newAlphaKeys[i] = new GradientAlphaKey(originalAlphaKeys[i].alpha, newTime);
                }

                Gradient newGradient = new Gradient();
                newGradient.SetKeys(colorKeys, newAlphaKeys);
                _trailRenderer.colorGradient = newGradient;
            }
        }
    }

    public override void Destroy()
    {
        ClearTelegraphIndicators();

        if (_instance != null)
        {
            Object.Destroy(_instance);
        }
    }

    private void SpawnSpear()
    {
        if (_visualConfig.SpearPrefab == null) return;

        Vector3 direction = (_spearEndPosition - _spearStartPosition).normalized;

        Quaternion rotation = CalculateSpearRotation(direction);

        Vector3 spawnPosition = _spearStartPosition;
        spawnPosition.y = _visualConfig.SpearGroundY;

        _instance = Object.Instantiate(
            _visualConfig.SpearPrefab,
            spawnPosition,
            rotation,
            _parent);

        _instance.name = GetSpearInstanceName();

        _renderer = _instance.GetComponent<Renderer>();
        if (_renderer != null && _renderer.material != null)
        {
            Color color = _renderer.material.color;
            color.a = 0f;
            _renderer.material.color = color;
        }

        InitializeCommitFlashIndicator();
        InitializeTrailRenderer();
        NotifyAttackSpawned();
    }

    private void InitializeTrailRenderer()
    {
        if (_instance == null) return;

        _trailRenderer = _instance.GetComponent<TrailRenderer>();
        if (_trailRenderer != null)
        {
            _trailRenderer.enabled = false;
        }
    }

    private void ClearTelegraphIndicators()
    {
        foreach (GameObject indicator in _telegraphIndicators)
        {
            if (indicator != null)
            {
                Object.Destroy(indicator);
            }
        }
        _telegraphIndicators.Clear();
    }
}