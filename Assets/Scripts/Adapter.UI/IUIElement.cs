using UnityEngine;

public interface IUIElement
{
    void Show();
    void Hide();
}

[System.Serializable]
public abstract class UIElementBase : MonoBehaviour, IUIElement
{
    public abstract void Hide();
    public abstract void Show();
}