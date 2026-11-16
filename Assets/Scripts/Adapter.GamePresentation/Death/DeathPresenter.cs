using System;
using Domain.Ports.Driving;
using UnityEngine;
using System.Collections;

public class DeathPresenter : MonoBehaviour
{
    [Header("Slow Motion Settings")]
    [Tooltip("Real seconds for game time to slow from full speed to a stop")]
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

    private ITimeService _timeService;
    private IPlayerService _playerService;
    private CameraEffectsPresenter _cameraEffectsPresenter;
    private PlayerPresenter _playerPresenter;

    /// <summary>
    /// Raised when the death sequence has played long enough for the game over screen.
    /// </summary>
    public event Action OnDeathSequenceFinished;

    public void Initialize(IPlayerService playerService, CameraEffectsPresenter cameraEffectsPresenter, ITimeService timeService, PlayerPresenter playerPresenter)
    {
        _playerService = playerService;
        _cameraEffectsPresenter = cameraEffectsPresenter;
        _timeService = timeService;
        _playerPresenter = playerPresenter;

        _playerService.OnPlayerDied += HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        // The clock drops this hold when the run ends, so it is never released here.
        _timeService.SlowTo(0f, _slowMotionDuration);
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