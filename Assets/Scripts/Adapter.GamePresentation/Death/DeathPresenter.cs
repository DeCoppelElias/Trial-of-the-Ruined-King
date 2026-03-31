using Domain.Ports;
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

    private DomainUpdater _domainUpdater;
    private IPlayerService _playerService;
    private CameraEffectsPresenter _cameraEffectsPresenter;
    private PlayerPresenter _playerPresenter;
    private UIManager _uiManager;

    public void Initialize(IPlayerService playerService, CameraEffectsPresenter cameraEffectsPresenter, DomainUpdater domainUpdater, PlayerPresenter playerPresenter, UIManager uiManager)
    {
        _playerService = playerService;
        _cameraEffectsPresenter = cameraEffectsPresenter;
        _domainUpdater = domainUpdater;
        _playerPresenter = playerPresenter;
        _uiManager = uiManager;

        _playerService.OnPlayerDied += HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        _domainUpdater.SetTimeScale(0, _slowMotionDuration);
        _cameraEffectsPresenter.ZoomAndRotateCamera(_zoomInDuration, _zoomOutDuration, _zoomAmount, _rotationAmount);
        _playerPresenter.PlayDeathAnimation(_deathFallDuration);

        StartCoroutine(ShowGameOverUIAfterDelay());
    }

    private IEnumerator ShowGameOverUIAfterDelay()
    {
        yield return new WaitForSecondsRealtime(_gameOverUIDelay);
        _uiManager.ShowGameOver(_playerService.Gold, _playerService.Score, _playerService.Score == _playerService.HighScore);
    }
}