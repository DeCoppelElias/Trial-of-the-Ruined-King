using Domain.Ports;
using UnityEngine;

public class CoinCreator : MonoBehaviour
{
    private ICoinService _coinService;
    private IGameStateService _gameStateService;

    private float _timeSinceLastCoin;
    [SerializeField] private float _coinCreateCooldown = 5f;
    [SerializeField] private float _minManhattanDistanceFromPlayer = 3f;

    public void Initialize(ICoinService coinService, IGameStateService gameStateService)
    {
        _coinService = coinService;
        _gameStateService = gameStateService;
    }
    
    public void HandleTick(float deltaTime)
    {
        if (_coinService.CoinCount == 0 && _gameStateService.CurrentState == GameState.Gameplay)
        {
            _timeSinceLastCoin += deltaTime;
            
            if (_timeSinceLastCoin >= _coinCreateCooldown)
            {
                _coinService.CreateCoinAtRandomPosition(_minManhattanDistanceFromPlayer);
                _timeSinceLastCoin = 0f;
            }
        }
    }
}