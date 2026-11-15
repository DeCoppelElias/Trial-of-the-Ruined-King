using System;
using Domain.Ports.Driving;
using UnityEngine;
using System.Collections;

public class DeathPresenter : MonoBehaviour
{
    [Header("Time Slow Settings")]
    [SerializeField] private float _slowMotionDuration = 5.0f;

    [Header("Death Animation Settings")]   
    [SerializeField] private float _deathFallDuration = 0.5f;

    [Header("Camera Effects Settings")]
    [SerializeField] private float _zoomInDuration = 2f;
    [SerializeField] private float _zoomOutDuration = 0.5f;
    [SerializeField] private float _zoomAmount = 10f;
    [SerializeField] private float _rotationAmount = 5f;

    [Header("Game Over UI Settings")]
    [SerializeField] private float _gameOverUIDelay = 2.5f;

    private Action<float, float> _setTimeScale;
    private IPlayerService _playerService;
    private CameraEffectsPresenter _cameraEffectsPresenter;
    private PlayerPresenter _playerPresenter;

    /// <summary>
    /// Raised when the death sequence has played long enough for the game over screen.
    /// </summary>
    public event Action OnDeathSequenceFinished;

    public void Initialize(IPlayerService playerService, CameraEffectsPresenter cameraEffectsPresenter, Action<float, float> setTimeScale, PlayerPresenter playerPresenter)
    {
        _playerService = playerService;
        _cameraEffectsPresenter = cameraEffectsPresenter;
        _setTimeScale = setTimeScale;
        _playerPresenter = playerPresenter;

        _playerService.OnPlayerDied += HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        _setTimeScale(0, _slowMotionDuration);
        _cameraEffectsPresenter.ZoomAndRotateCamera(_zoomInDuration, _zoomOutDuration, _zoomAmount, _rotationAmount);
        _playerPresenter.PlayDeathAnimation(_deathFallDuration);

        StartCoroutine(ShowGameOverUIAfterDelay());
    }

    private IEnumerator ShowGameOverUIAfterDelay()
    {
        yield return new WaitForSecondsRealtime(_gameOverUIDelay);
        OnDeathSequenceFinished?.Invoke();
    }
}