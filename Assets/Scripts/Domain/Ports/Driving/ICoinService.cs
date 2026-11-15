using System;

namespace Domain.Ports.Driving
{
    public interface ICoinService
    {
        void CreateCoinAtRandomPosition(float minManhattanDistanceFromPlayer);
        int CoinCount { get; }

        event Action<Coin> OnCoinCreated;
        event Action<Coin> OnCoinRemoved;
    }
}
