using Domain.Ports;
using System;
using System.Linq;

public class CoinService : ICoinService
{
    private readonly IArena _arena;
    private readonly IPlayerService _playerService;
    private readonly IGameStateService _gameStateService;
    private readonly ILogger _logger;
    private Coin _coin;
    
    public int CoinCount => _coin != null ? 1 : 0;

    public event Action<Coin> OnCoinCreated;
    public event Action<Coin> OnCoinRemoved;

    public CoinService(ILogger logger, IArena arena, IPlayerService playerService, IGameStateService gameStateService)
    {
        _arena = arena;
        _playerService = playerService;
        _gameStateService = gameStateService;
        _logger = logger;

        _playerService.OnPlayerMove += HandlePlayerMoved;
        _gameStateService.OnStateChanged += HandleStateChanged;
    }

    public void CreateCoinAtRandomPosition(float minManhattanDistanceFromPlayer)
    {
        var playerPosition = _playerService.GetPlayerGridPosition();
        var allTiles = _arena.GetAllTiles();

        var validPositions = allTiles
            .Where(tile => tile.tileType == TileType.Floor)
            .Where(tile => CalculateManhattanDistance(playerPosition, tile.position) >= minManhattanDistanceFromPlayer)
            .Select(tile => tile.position)
            .ToList();

        if (validPositions.Count == 0)
        {
            _logger.LogWarning("No valid positions available to spawn a coin.");
            return;
        }

        var randomIndex = UnityEngine.Random.Range(0, validPositions.Count);
        var randomPosition = validPositions[randomIndex];

        _coin = new Coin(randomPosition);
        OnCoinCreated?.Invoke(_coin);
    }

    private float CalculateManhattanDistance(GridPosition pos1, GridPosition pos2)
    {
        var dx = Math.Abs(pos1.GridX - pos2.GridX);
        var dy = Math.Abs(pos1.GridY - pos2.GridY);
        return dx + dy;
    }

    private void HandlePlayerMoved(GridPosition _, GridPosition gridPosition)
    {
        if (_coin == null) return;

        if (gridPosition.Equals(_coin.Position))
        {
            var collectedCoin = _coin;
            _coin = null;
            
            _playerService.GivePlayerGold(1);
            
            OnCoinRemoved?.Invoke(collectedCoin);
        }
    }

    private void HandleStateChanged(GameState previousState, GameState newState)
    {
        if (previousState == GameState.Gameplay && newState != GameState.Gameplay)
        {
            HandleExitGameplay();
        }
    }

    private void HandleExitGameplay()
    {
        if (_coin != null)
        {
            _coin = null;
            OnCoinRemoved?.Invoke(_coin);
        }
    }
}