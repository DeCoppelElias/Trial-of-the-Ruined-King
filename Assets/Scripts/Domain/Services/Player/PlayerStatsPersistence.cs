using Domain.Ports;

public class PlayerStatsPersistence : IPlayerStatsPersistence
{
    private readonly IPlayerService _playerService;
    private readonly IStorage _storage;
    public PlayerStatsPersistence(IPlayerService playerService, IStorage storage)
    {
        _playerService = playerService;
        _storage = storage;
    }

    public void HandleExitGameplay()
    {
        _storage.Save(StorageKeys.Highscore.ToString(), _playerService.HighScore);
        _storage.Save(StorageKeys.Gold.ToString(), _playerService.Gold);
    }

    public void HandleStartGame()
    {
        if (_storage.HasKey(StorageKeys.Highscore.ToString()))
        {
            _playerService.HighScore = _storage.Load<int>(StorageKeys.Highscore.ToString());
        }
        if (_storage.HasKey(StorageKeys.Gold.ToString()))
        {
            _playerService.Gold = _storage.Load<int>(StorageKeys.Gold.ToString());
        }
    }
}