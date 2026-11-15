using Domain.Models;
using System;

namespace Domain.Ports.Driving
{
    public interface IGameStateService
    {
        GameState CurrentState { get; }
        event Action<GameState, GameState> OnStateChanged;
        void TransitionTo(GameState newState);
    }
}
