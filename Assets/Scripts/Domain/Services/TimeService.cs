using System;
using Domain.Ports;

namespace Domain.Services
{
    public class TimeService : ITimeService
    {
        public event Action<float> OnTick;
        
        public bool IsPaused { get; private set; }
        public float TotalElapsedTime { get; private set; }

        public TimeService(IGameStateService gameStateService)
        {
            IsPaused = false;
            TotalElapsedTime = 0f;
            gameStateService.OnStateChanged += OnGameStateChanged;
        }

        public void Tick(float deltaTime)
        {
            if (IsPaused) return;
            
            OnTick?.Invoke(deltaTime);
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }

        private void OnGameStateChanged(GameState previousState, GameState newState)
        {
            if (newState == GameState.MainMenu)
            {
                Pause();
            }
            else if (newState == GameState.Gameplay)
            {
                Resume();
            }
        }
    }
}