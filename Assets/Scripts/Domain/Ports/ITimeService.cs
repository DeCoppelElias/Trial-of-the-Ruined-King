using System;

namespace Domain.Ports
{
    public interface ITimeService
    {
        event Action<float> OnTick;
        
        void Tick(float deltaTime);
        
        bool IsPaused { get; }
        void Pause();
        void Resume();
        
        float TotalElapsedTime { get; }
    }
}