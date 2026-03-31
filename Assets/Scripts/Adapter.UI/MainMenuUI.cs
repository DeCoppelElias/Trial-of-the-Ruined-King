using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : UIElementBase
{
    [SerializeField] private GameObject _container;
    [SerializeField] private Button _startButton;

    private GameStateService _gameStateManager;

    private void Awake()
    {
        if (_container == null)
        {
            _container = gameObject;
        }
    }

    public void Initialize(GameStateService gameStateManager)
    {
        _gameStateManager = gameStateManager;

        if (_startButton != null)
        {
            _startButton.onClick.AddListener(OnStartButtonClicked);
        }
    }

    private void OnStartButtonClicked()
    {
        _gameStateManager.TransitionTo(GameState.Gameplay);
    }

    public override void Show()
    {
        _container.SetActive(true);
    }

    public override void Hide()
    {
        _container.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_startButton != null)
        {
            _startButton.onClick.RemoveListener(OnStartButtonClicked);
        }
    }
}