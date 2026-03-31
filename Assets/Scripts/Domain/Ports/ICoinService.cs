
using System;

public interface ICoinService
{
    void CreateCoinAtRandomPosition(float minManhattanDistanceFromPlayer);
    int CoinCount { get; }

    event Action<Coin> OnCoinCreated;
    event Action<Coin> OnCoinRemoved;
}