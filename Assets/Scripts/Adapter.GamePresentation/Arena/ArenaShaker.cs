using Domain.Models;
using Domain.Ports.Driving;
using UnityEngine;

public class ArenaShaker : MonoBehaviour
{
    [SerializeField] private Transform _arenaParent;

    private ITimeService _timeService;
    private RandomSource _cosmeticRandom;
    private Vector3 _originalPosition;
    private float _shakeDuration;
    private float _shakeMagnitude;
    private float _shakeElapsed;

    public void Initialize(ITimeService timeService, RandomSource cosmeticRandom)
    {
        _timeService = timeService;
        _cosmeticRandom = cosmeticRandom;

        if (_arenaParent != null)
            _originalPosition = _arenaParent.localPosition;

        if (_timeService != null)
            _timeService.OnTick += HandleTick;
    }

    private void OnDestroy()
    {
        if (_timeService != null)
            _timeService.OnTick -= HandleTick;
    }

    public void Shake(float duration, float magnitude)
    {
        _shakeDuration = duration;
        _shakeMagnitude = magnitude;
        _shakeElapsed = 0f;
    }

    private void HandleTick(float deltaTime)
    {
        if (_shakeElapsed < _shakeDuration)
        {
            _shakeElapsed += deltaTime;
            float x = _cosmeticRandom.Range(-1f, 1f) * _shakeMagnitude;
            float y = _cosmeticRandom.Range(-1f, 1f) * _shakeMagnitude;
            if (_arenaParent != null)
                _arenaParent.localPosition = _originalPosition + new Vector3(x, y, 0f);

            if (_shakeElapsed >= _shakeDuration && _arenaParent != null)
                _arenaParent.localPosition = _originalPosition;
        }
    }
}