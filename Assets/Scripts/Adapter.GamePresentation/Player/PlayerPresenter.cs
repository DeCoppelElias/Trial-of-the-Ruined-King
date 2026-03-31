using Domain.Models.Player;
using Domain.Ports;
using System;
using System.Collections;
using UnityEngine;

public class PlayerPresenter : MonoBehaviour
{
    private GameObject _playerInstance;
    private Player _player;
    private IArena _arena;

    [SerializeField] private GameObject _playerPrefab;

    private ITimeService _timeService;
    private IPlayerService _playerService;
    private DomainUpdater _domainUpdater;

    private Coroutine _aliveRoutine;
    private Vector3 _originalScale;

    [Header("Player Movement Settings")]
    [SerializeField] private readonly float _windUpDelay = 0.05f;
    [SerializeField] private readonly float _dodgeDuration = 0.15f;
    [SerializeField] private readonly float _overshootAmount = 0.1f;
    [SerializeField] private readonly float _visualYOffset = 0.6f;

    [Header("Player Spawn Settings")]
    [SerializeField] private float _fadeInDuration = 0.5f;


    public event Action<GameObject> OnPlayerSpawned;

    public void Initialize(Player player, IArena arena, GameObject playerPrefab, ArenaPresenter arenaPresenter, IGameStateService gameStateService, ITimeService timeService, IPlayerService playerService, DomainUpdater domainUpdater)
    {
        _player = player;
        _arena = arena;
        _playerPrefab = playerPrefab;
        _timeService = timeService;
        _playerService = playerService;
        _domainUpdater = domainUpdater;

        arenaPresenter.OnArenaVisualized += OnArenaVisualized;
        gameStateService.OnStateChanged += OnStateChange;
        _playerService.OnPlayerMove += OnPlayerMove;
    }

    private void OnStateChange(GameState previousState, GameState newState)
    {
        if (newState == GameState.MainMenu)
        {
            DestroyPlayer();
        }
    }

    private void OnArenaVisualized()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        GridPosition middleGridPosition = GetMiddleGridPosition();

        Vector3 playerStartPosition = _arena.GridToWorld(middleGridPosition) + new Vector3(0, _visualYOffset, 0);

        _playerInstance = Instantiate(_playerPrefab);
        _playerInstance.transform.position = playerStartPosition;

        _originalScale = _playerInstance.transform.localScale;

        SetPlayerAlpha(0f);
        StartCoroutine(FadeInPlayer(_fadeInDuration));

        OnPlayerSpawned?.Invoke(_playerInstance);
    }

    private void DestroyPlayer()
    {
        if (_playerInstance != null)
        {
            Destroy(_playerInstance);
            _playerInstance = null;
        }
    }

    private GridPosition GetMiddleGridPosition()
    {
        int middleX = (_arena.Width - 1) / 2;
        int middleY = (_arena.Height - 1) / 2;
        return new GridPosition(middleX, middleY);
    }

    private void OnPlayerMove(GridPosition oldPosition, GridPosition newPosition)
    {
        if (_playerInstance is null) return;

        Vector3 oldWorld = _arena.GridToWorld(oldPosition) + new Vector3(0, _visualYOffset, 0);
        Vector3 newWorld = _arena.GridToWorld(newPosition) + new Vector3(0, _visualYOffset, 0);
        Vector3 moveDir = (newWorld - oldWorld).normalized;

        if (_aliveRoutine != null)
            StopCoroutine(_aliveRoutine);

        _aliveRoutine = StartCoroutine(AnimateMoveWithOvershoot(oldWorld, newWorld, moveDir));
    }

    public void PlayDeathAnimation(float deathFallDuration)
    {
        if (_playerInstance == null) return;

        if (_aliveRoutine != null)
            StopCoroutine(_aliveRoutine);

        StartCoroutine(AnimateDeathFall(deathFallDuration));
    }

    private IEnumerator AnimateMoveWithOvershoot(Vector3 startPosition, Vector3 targetPosition, Vector3 direction)
    {
        // Wind-up phase
        float windUpElapsed = 0f;
        while (windUpElapsed < _windUpDelay)
        {
            windUpElapsed += Time.deltaTime * _domainUpdater.CurrentTimeScale;
            yield return null;
        }

        // Calculate overshoot position
        Vector3 overshootPosition = targetPosition + direction * _overshootAmount;

        // Dodge phase: forward with overshoot
        float elapsed = 0f;
        float dodgePhase = _dodgeDuration * 0.65f;
        while (elapsed < dodgePhase)
        {
            elapsed += Time.deltaTime * _domainUpdater.CurrentTimeScale;
            float t = Mathf.Clamp01(elapsed / dodgePhase);
            t = 1f - (1f - t) * (1f - t); // Ease-out

            Vector3 currentPosition = Vector3.Lerp(startPosition, overshootPosition, t);
            _playerInstance.transform.position = currentPosition;

            AnimateAliveVisual(direction, t);

            yield return null;
        }

        // Snap back phase: overshoot to target
        elapsed = 0f;
        float snapBackPhase = _dodgeDuration * 0.35f;
        while (elapsed < snapBackPhase)
        {
            elapsed += Time.deltaTime * _domainUpdater.CurrentTimeScale;
            float t = Mathf.Clamp01(elapsed / snapBackPhase);
            t = t * t; // Ease-in

            Vector3 currentPosition = Vector3.Lerp(overshootPosition, targetPosition, t);
            _playerInstance.transform.position = currentPosition;

            AnimateAliveVisual(direction, t);

            yield return null;
        }

        // Ensure exact final position
        _playerInstance.transform.position = targetPosition;
        _playerInstance.transform.localScale = _originalScale;

        // Small landing bounce
        yield return StartCoroutine(LandingBounce());
    }

    private void AnimateAliveVisual(Vector3 moveDirection, float t)
    {
        Quaternion startRot = _playerInstance.transform.rotation;
        Quaternion targetRot = moveDirection != Vector3.zero ? Quaternion.LookRotation(moveDirection) : startRot;
        _playerInstance.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

        float stretchAmount = 0.15f;
        float pulse = Mathf.Sin(t * Mathf.PI);

        Vector3 stretchScale = new Vector3(
            1f + moveDirection.x * stretchAmount * pulse,
            1f - stretchAmount * pulse,
            1f + moveDirection.z * stretchAmount * pulse
        );

        _playerInstance.transform.localScale = new Vector3(
            stretchScale.x * _originalScale.x,
            stretchScale.y * _originalScale.y,
            stretchScale.z * _originalScale.z
        );
    }

    private IEnumerator LandingBounce()
    {
        float bounceDuration = 0.1f;
        float timer = 0f;

        Vector3 startPos = _playerInstance.transform.position;

        while (timer < bounceDuration)
        {
            timer += Time.deltaTime * _domainUpdater.CurrentTimeScale;
            float t = timer / bounceDuration;

            float bounce = Mathf.Sin(t * Mathf.PI) * 0.1f;
            _playerInstance.transform.position = startPos + Vector3.up * bounce;

            yield return null;
        }

        _playerInstance.transform.position = startPos;
    }

    private IEnumerator AnimateDeathFall(float deathFallDuration)
    {
        Quaternion startRotation = _playerInstance.transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(-90f, 0f, 0f);
        
        float elapsed = 0f;
        
        while (elapsed < deathFallDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / deathFallDuration);
            
            float easedT = 1f - Mathf.Pow(1f - t, 3f);
            
            _playerInstance.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, easedT);
            
            yield return null;
        }
        
        _playerInstance.transform.rotation = targetRotation;
    }

    private IEnumerator FadeInPlayer(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime * _domainUpdater.CurrentTimeScale;
            float alpha = Mathf.Clamp01(elapsed / duration);
            SetPlayerAlpha(alpha);
            yield return null;
        }
        SetPlayerAlpha(1f);
    }

    private void SetPlayerAlpha(float alpha)
    {
        if (_playerInstance == null) return;

        var renderers = _playerInstance.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color color = mat.color;
                    color.a = alpha;
                    mat.color = color;
                    // Ensure material is set to transparent mode
                    if (mat.HasProperty("_Mode"))
                        mat.SetFloat("_Mode", 2f); // 2 = Transparent in Unity Standard Shader
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3000;
                }
            }
        }
    }
}