using Domain.Ports;
using UnityEngine;

public class DomainUpdater : MonoBehaviour
{
    private ITimeService _timeService;
    private IGameStateService _gameStateService;
    private bool _initialized;
    [SerializeField] private float _baseTimeScale = 1.0f;

    [SerializeField] private float _currentTimeScale;
    private float _targetTimeScale;
    private float _transitionDuration;
    private float _transitionTimer;
    private bool _isTransitioning;

    public float CurrentTimeScale => _currentTimeScale;

    public void Initialize(ITimeService timeService, IGameStateService gameStateService)
    {
        _timeService = timeService;
        _gameStateService = gameStateService;
        _initialized = true;
        _currentTimeScale = _baseTimeScale;
        _targetTimeScale = _baseTimeScale;

        _gameStateService.OnStateChanged += (prev, next) =>
        {
            if (next == GameState.Gameplay)
            {
                ResetTimeScale();
            }
        };
    }

    public void SetTimeScale(float targetScale, float duration = 0f)
    {
        if (duration <= 0f)
        {
            _currentTimeScale = targetScale;
            _targetTimeScale = targetScale;
            _isTransitioning = false;
        }
        else
        {
            _targetTimeScale = targetScale;
            _transitionDuration = duration;
            _transitionTimer = 0f;
            _isTransitioning = true;
        }
    }

    public void ResetTimeScale()
    {
        SetTimeScale(_baseTimeScale);
    }

    private void Update()
    {
        if (!_initialized) return;

        if (_isTransitioning)
        {
            _transitionTimer += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(_transitionTimer / _transitionDuration);
            
            _currentTimeScale = Mathf.Lerp(_currentTimeScale, _targetTimeScale, progress);
            
            if (progress >= 1f)
            {
                _currentTimeScale = _targetTimeScale;
                _isTransitioning = false;
            }
        }

        _timeService.Tick(Time.deltaTime * _currentTimeScale);
    }
}
