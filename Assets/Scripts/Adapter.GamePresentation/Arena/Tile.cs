using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Represents a single tile in the arena with visual indicators for danger warnings.
/// </summary>
public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject _dangerIndicator;

    private void Awake()
    {
        InitializeDangerIndicator();
    }

    private void InitializeDangerIndicator()
    {
        // Find the danger indicator child if not assigned
        if (_dangerIndicator == null)
        {
            Transform dangerTransform = transform.Find("Danger Indicator");
            if (dangerTransform != null)
            {
                _dangerIndicator = dangerTransform.gameObject;
            }
            else
            {
                Debug.LogWarning($"TileVisualizer on {gameObject.name}: No 'Danger Indicator' child found.");
                return;
            }
        }

        // Disable danger indicator on startup
        _dangerIndicator.SetActive(false);
    }

    /// <summary>
    /// Shows the danger indicator on this tile.
    /// Called by AttackVisualizer when this tile becomes dangerous.
    /// </summary>
    public void ShowDangerIndicator()
    {
        if (_dangerIndicator != null)
        {
            _dangerIndicator.SetActive(true);
        }
    }

    /// <summary>
    /// Hides the danger indicator on this tile.
    /// Called by AttackVisualizer when danger passes.
    /// </summary>
    public void HideDangerIndicator()
    {
        if (_dangerIndicator != null)
        {
            _dangerIndicator.SetActive(false);
        }
    }

    /// <summary>
    /// Gets the current state of the danger indicator.
    /// </summary>
    public bool IsDangerIndicatorActive()
    {
        return _dangerIndicator != null && _dangerIndicator.activeSelf;
    }
}