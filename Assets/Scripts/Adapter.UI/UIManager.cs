using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private Dictionary<Type, UIElementBase> _uiElements;
    private ILogger _logger;

    public void Initialize(ILogger logger, List<UIElementBase> uiElementList, IGameStateService gameStateService)
    {
        _logger = logger;
        _uiElements = new Dictionary<Type, UIElementBase>();

        foreach (UIElementBase element in uiElementList)
        {
            Type elementType = element.GetType();
            _uiElements[elementType] = element;
        }

        gameStateService.OnStateChanged += OnGameStateChanged;
    }

    public void Show<T>() where T : UIElementBase
    {
        if (TryGetElement<T>(out UIElementBase element))
        {
            element.Show();
        }
    }

    public void Hide<T>() where T : UIElementBase
    {
        if (TryGetElement<T>(out UIElementBase element))
        {
            element.Hide();
        }
    }

    public void ShowGameOver(int collectedGold, int score, bool isNewHighscore)
    {
        if (TryGetElement<GameOverUI>(out UIElementBase element))
        {
            GameOverUI gameOverUI = element as GameOverUI;
            gameOverUI.SetStats(collectedGold, score, isNewHighscore);
            gameOverUI.Show();
        }
        
        Hide<MainMenuUI>();
    }

    public void ShowMainMenu()
    {
        Hide<GamePlayUI>();
        Show<MainMenuUI>();
        Hide<GameOverUI>();
    }

    public void ShowGamePlay()
    {
        Hide<MainMenuUI>();
        Hide<GameOverUI>();
        Show<GamePlayUI>();
    }

    private bool TryGetElement<T>(out UIElementBase element) where T : UIElementBase
    {
        Type elementType = typeof(T);
        
        if (_uiElements.TryGetValue(elementType, out element))
        {
            return true;
        }

        _logger.LogError($"UI element of type {elementType.Name} not found in UIManager.");
        element = null;
        return false;
    }

    private void OnGameStateChanged(GameState previousState, GameState newState)
    {
        if (newState == GameState.MainMenu)
        {
            ShowMainMenu();
        }
        else if (newState == GameState.Gameplay)
        {
            ShowGamePlay();
        }
    }
}