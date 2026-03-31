using UnityEngine;

public class GamePlayUI : UIElementBase
{
    [SerializeField] private CanvasGroup _container;

    public override void Show()
    {
        _container.alpha = 1f;
        _container.interactable = true;
        _container.blocksRaycasts = true;
    }

    public override void Hide()
    {
        _container.alpha = 0f;
        _container.interactable = false;
        _container.blocksRaycasts = false;
    }
}