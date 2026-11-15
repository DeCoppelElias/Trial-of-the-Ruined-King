namespace Domain.Ports.Driving
{
    public interface IPlayerStatsPersistence
    {
        void HandleExitGameplay();
        void HandleStartGame();
    }
}
