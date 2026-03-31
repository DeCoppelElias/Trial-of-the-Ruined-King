using System;

public interface IGameStateService
{
    GameState CurrentState { get; }
    event Action<GameState, GameState> OnStateChanged;
    void TransitionTo(GameState newState);
}