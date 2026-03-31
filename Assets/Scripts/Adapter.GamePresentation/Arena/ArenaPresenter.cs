using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArenaPresenter : MonoBehaviour
{
    [SerializeField] private Transform _arenaParent;
    [SerializeField] private GameObject _lightTilePrefab;
    [SerializeField] private GameObject _darkTilePrefab;

    [Header("Sequential Arena Animation Settings")]
    [SerializeField] private float _spawnDelay = 0.2f;
    [SerializeField] private float _riseDuration = 0.3f;
    [SerializeField] private float _fadeDuration = 0.3f;

    private IArena _arena;
    private bool _isInitialized;
    private Dictionary<GridPosition, GameObject> _tileInstances;

    public event Action OnArenaVisualized;

    public void Initialize(IArena arena, IGameStateService gameStateService)
    {
        _tileInstances = new Dictionary<GridPosition, GameObject>();
        _arena = arena;
        _arena.OnArenaChanged += Visualize;
        _isInitialized = true;

        gameStateService.OnStateChanged += OnStateChange;
    }

    private void OnDestroy()
    {
        if (_arena != null)
        {
            _arena.OnArenaChanged -= Visualize;
        }

        ClearAllTiles();
    }

    private void Visualize()
    {
        if (!_isInitialized || _arena == null || _lightTilePrefab == null || _darkTilePrefab == null) return;

        ClearAllTiles();

        foreach (var (position, tileType) in _arena.GetAllTiles())
        {
            if (tileType == TileType.Empty)
            {
                continue;
            }

            Vector3 localPosition = new Vector3(position.GridX, 0f, position.GridY);
            Vector3 worldPosition = _arena.WorldPosition + localPosition;

            int i = position.GridX + position.GridY;
            GameObject prefabToUse = (i % 2 == 0) ? _lightTilePrefab : _darkTilePrefab;
            GameObject tileInstance = Instantiate(prefabToUse, worldPosition, Quaternion.identity, _arenaParent);
            tileInstance.name = $"Tile_{position.GridX}_{position.GridY}";
            
            _tileInstances[position] = tileInstance;
        }

        OnArenaVisualized?.Invoke();
    }

    private void ClearAllTiles()
    {
        foreach (var tileInstance in _tileInstances.Values)
        {
            if (tileInstance != null)
            {
                Destroy(tileInstance);
            }
        }

        _tileInstances.Clear();
    }

    private void OnStateChange(GameState oldState, GameState newState)
    {
        if (newState == GameState.Gameplay)
        {
            VisualizeArenaIncrementally();
        }
        else ClearAllTiles();
    }

    public void VisualizeArenaIncrementally()
    {
        if (!_isInitialized || _arena == null || _lightTilePrefab == null || _darkTilePrefab == null) return;

        ClearAllTiles();

        var tiles = _arena.GetAllTiles()
            .Where(t => t.tileType != TileType.Empty)
            .Select(t => t.position)
            .ToList();

        // Find center tile: lowest GridY (Z), then WorldX closest to 0
        int minY = tiles.Min(p => p.GridY);
        var lowestYTiles = tiles.Where(p => p.GridY == minY).ToList();
        var orderedByX = lowestYTiles.OrderBy(p => p.GridX).ToList();
        var centerTile = orderedByX[orderedByX.Count / 2];

        StartCoroutine(BuildTilesCoroutine(centerTile, tiles, _spawnDelay, _riseDuration, _fadeDuration));
    }

    private IEnumerator BuildTilesCoroutine(GridPosition centerTile, List<GridPosition> tiles, float spawnDelay, float riseDuration, float fadeDuration)
    {
        // Group tiles by Manhattan distance from center
        var tilesByDistance = tiles
            .GroupBy(p => Math.Abs(p.GridY - centerTile.GridY) + Math.Abs(p.GridX - centerTile.GridX))
            .OrderBy(g => g.Key);

        foreach (var group in tilesByDistance)
        {
            foreach (var position in group)
            {
                Vector3 localPosition = new Vector3(position.GridX, 0, position.GridY);
                Vector3 worldPosition = _arena.WorldPosition + localPosition;

                int i = position.GridX + position.GridY;
                GameObject prefabToUse = (i % 2 == 0) ? _lightTilePrefab : _darkTilePrefab;
                GameObject tileInstance = Instantiate(prefabToUse, worldPosition + new Vector3(0, -0.5f, 0), Quaternion.identity, _arenaParent);
                tileInstance.name = $"Tile_{position.GridX}_{position.GridY}";

                _tileInstances[position] = tileInstance;

                StartCoroutine(AnimateTileSpawnMovement(tileInstance, worldPosition, riseDuration));
                StartCoroutine(AnimateTileSpawnColor(tileInstance, 1f, fadeDuration));
            }

            yield return new WaitForSeconds(spawnDelay);
        }

        OnArenaVisualized?.Invoke();
    }

    private IEnumerator AnimateTileSpawnColor(GameObject tile, float targetAlpha, float fadeDuration)
    {
        float elapsed = 0f;

        var renderer = tile.GetComponentInChildren<Renderer>();
        if (renderer != null && renderer.material.HasProperty("_Color"))
        {
            Color color = renderer.material.color;

            while (elapsed < fadeDuration)
            {
                float t = elapsed / fadeDuration;
                color.a = Mathf.Lerp(color.a, targetAlpha, t);
                renderer.material.color = color;
                elapsed += Time.deltaTime;
                yield return null;
            }

            color.a = targetAlpha;
            renderer.material.color = color;
        }
    }

    private IEnumerator AnimateTileSpawnMovement(GameObject tile, Vector3 targetPosition, float movementDuration)
    {
        float elapsed = 0f;

        while (elapsed < movementDuration)
        {
            float t = elapsed / movementDuration;
            tile.transform.position = Vector3.Lerp(tile.transform.position, targetPosition, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        tile.transform.position = targetPosition;
    }
}
