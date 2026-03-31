using Domain.Ports;
using System.Collections;
using UnityEngine;

public class PlayerHitPresenter : MonoBehaviour
{
    private ITimeService _timeService;
    private IPlayerService _playerService;

    [Header("Hit Time Pause Settings")]
    [SerializeField] private float _timePauseDuration;

    [Header("Hit Audio Settings")]
    [SerializeField] private AudioPlayer _audioPlayer;
    [SerializeField] private AudioEvent _hitAudio;

    [Header("Hit Camera Shake Settings")]
    [SerializeField] private CameraEffectsPresenter _cameraShaker;
    [SerializeField] private float _cameraShakeDuration;
    [SerializeField] private float _cameraShakeMagnitude;

    public void Initialize(ITimeService timeService, IPlayerService playerService)
    {
        _timeService = timeService;
        _playerService = playerService;

        _playerService.OnPlayerAttackHit += HandlePlayerAttackHit;
    }

    private void HandlePlayerAttackHit()
    {
        _audioPlayer.Play(_hitAudio);
        _cameraShaker.ShakeCamera(_cameraShakeDuration, _cameraShakeMagnitude);
        StartCoroutine(PauseCoroutine(_timePauseDuration));
    }

    private IEnumerator PauseCoroutine(float duration)
    {
        _timeService.Pause();
        yield return new WaitForSeconds(duration);
        _timeService.Resume();
    }
}
