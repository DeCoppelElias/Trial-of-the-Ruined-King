using Domain.Models;
using Domain.Ports.Driving;
using System;

namespace Domain.Services
{
    public class GameStateService : IGameStateService
    {
        public GameState CurrentState { get; private set; } = GameState.MainMenu;

        public event Action<GameState, GameState> OnStateChanged;

        public void TransitionTo(GameState newState)
        {
            if (CurrentState == newState) return;

            GameState previousState = CurrentState;
            CurrentState = newState;
            OnStateChanged?.Invoke(previousState, newState);
        }
    }
}
