using Domain.Models.Player;
using Domain.Ports;
using Domain.Services;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;

    [Header("Gameplay")]
    [SerializeField] private PlayerAttacker _playerAttacker;
    [SerializeField] private CoinCreator _coinCreator;
    [SerializeField] private DomainUpdater _domainUpdater;

    [Header("Visualizers and Presenters")]
    [SerializeField] private ArenaPresenter _arenaPresenter;
    [SerializeField] private TileVisualizer _tileVisualizer;
    [SerializeField] private AttackPresenter _attackPresenter;
    [SerializeField] private PlayerHitPresenter _playerHitPresenter;
    [SerializeField] private PlayerPresenter _playerPresenter;
    [SerializeField] private CoinsPresenter _coinsPresenter;
    [SerializeField] private DeathPresenter _deathPresenter;
    [SerializeField] private CameraEffectsPresenter _cameraEffectsPresenter;

    [Header("UI")]
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private MainMenuUI _mainMenuUI;
    [SerializeField] private GamePlayUI _gamePlayUI;

    [SerializeField] private GameOverUI _gameOverUI;

    [SerializeField] private PlayerStatsUI _scoreUI;
    [SerializeField] private PlayerStatsUI _highscoreUI;
    [SerializeField] private PlayerStatsUI _goldUI;

    [Header("Audio")]
    [SerializeField] private AudioPlayer _audioPlayer;

    [Header("Shakers")]
    [SerializeField] private ArenaShaker _arenaShaker;

    [Header("Debug")]
    [SerializeField] private bool _initializeDebugComponents;
    [SerializeField] private DebugAttackPresenter _debugAttackPresenter;

    [SerializeField] private int _arenaWidth = 3;
    [SerializeField] private int _arenaHeight = 4;
    [SerializeField] private Vector3 _arenaPosition = new (-1, 0, -2);

    private RectangleArena _arena;
    private ITimeService _timeService;
    private AttackService _attackService;
    private PlayerService _playerService;
    private PlayerStatsPersistence _playerPersistence;
    private Player _player;
    private ILogger _logger;
    private GameStateService _gameStateService;
    private CoinService _coinService;
    private IStorage _storage;

    private void Awake()
    {
        // Domain => can directly use other domain services
        // Adapters => Listen to events from domain
        // Adapters => Can directly use domain

        _logger = new UnityLogger();

        _gameStateService = new GameStateService();
        _arena = new RectangleArena(_arenaWidth, _arenaHeight, _arenaPosition);
        _player = new Player(1, GetMiddleGridPosition(), _logger);
        _timeService = new TimeService(_gameStateService);
        _attackService = new AttackService(_timeService, _gameStateService);
        _storage = new PlayerPrefsStorage();
        InitializePlayerComponents();
        InitializeCoinComponents();

        _domainUpdater.Initialize(_timeService, _gameStateService);
        _tileVisualizer.Initialize(_arena, _arenaPresenter);
        _attackPresenter.Initialize(_attackService, _arena, _timeService, _domainUpdater);
        _arenaPresenter.Initialize(_arena, _gameStateService);
        _playerHitPresenter.Initialize(_timeService, _playerService);
        _arenaShaker.Initialize(_timeService);
        _playerAttacker.Initialize(_attackService, _arena, _timeService, _gameStateService);
        _playerPresenter.Initialize(_player, _arena, _playerPrefab, _arenaPresenter, _gameStateService, _timeService, _playerService, _domainUpdater);
        _playerPresenter.OnPlayerSpawned += OnPlayerSpawned;
        _deathPresenter.Initialize(_playerService, _cameraEffectsPresenter, _domainUpdater, _playerPresenter, _uiManager);

        InitializeUIComponents();
        InitializeDebugComponents();

        _timeService.Pause();
        _uiManager.ShowMainMenu();
    }

    private void OnPlayerSpawned(GameObject obj)
    {
        var movementComponent = obj.GetComponent<PlayerMovement>();
        if (movementComponent == null) movementComponent = obj.AddComponent<PlayerMovement>();

        movementComponent.Initialize(_playerService);
    }

    private GridPosition GetMiddleGridPosition()
    {
        int middleX = (_arena.Width - 1) / 2;
        int middleY = (_arena.Height - 1) / 2;
        return new GridPosition(middleX, middleY);
    }

    private void InitializeUIComponents()
    {
        List<UIElementBase> uiElements = new List<UIElementBase>
        {
            _mainMenuUI,
            _gamePlayUI,
            _scoreUI,
            _gameOverUI
        };

        _mainMenuUI.Initialize(_gameStateService);
        _scoreUI.Initialize(_playerService.Score);
        _highscoreUI.Initialize(_playerService.HighScore);
        _goldUI.Initialize(_playerService.Gold);
        _gameOverUI.Initialize(_gameStateService);
        _uiManager.Initialize(_logger, uiElements, _gameStateService);

        _playerService.OnPlayerScoreChange += newScore => _scoreUI.HandleUpdate(newScore);
        _playerService.OnPlayerHighScoreChange += newHighscore => _highscoreUI.HandleUpdate(newHighscore);
        _playerService.OnPlayerGoldChange += newGold => _goldUI.HandleUpdate(newGold);
    }

    private void InitializeCoinComponents()
    {
        _coinService = new CoinService(_logger, _arena, _playerService, _gameStateService);

        _coinCreator.Initialize(_coinService, _gameStateService);
        _coinsPresenter.Initialize(_arena, _audioPlayer);

        _coinService.OnCoinCreated += _coinsPresenter.HandleCoinCreated;
        _coinService.OnCoinRemoved += _coinsPresenter.HandleCoinRemoved;
        _timeService.OnTick += _coinCreator.HandleTick;
    }

    private void InitializeDebugComponents()
    {
        if (!_initializeDebugComponents) return;

        _debugAttackPresenter.Initialize(_attackService, _arena);
    }

    private void Start()
    {
        _playerPersistence.HandleStartGame();
    }

    private void InitializePlayerComponents()
    {
        _playerService = new PlayerService(_player, _arena, _attackService, _timeService, _logger);
        _playerPersistence = new PlayerStatsPersistence(_playerService, new PlayerPrefsStorage());

        _gameStateService.OnStateChanged += (prev, next) =>
        {
            if (next == GameState.Gameplay)
            {
                _playerService.ResetPlayer();
            }
            if (prev == GameState.Gameplay && next != GameState.Gameplay)
            {
                _playerPersistence.HandleExitGameplay();
            }
        };
    }
}
