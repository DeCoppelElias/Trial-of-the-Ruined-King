using System.Collections;
using UnityEngine;

/// <summary>
/// Automatically disables itself after flashing for a specified duration.
/// </summary>
public class FlashIndicator : MonoBehaviour
{
    [SerializeField] private float _flashDuration = 0.2f;

    private Coroutine _flashCoroutine;


    private void OnEnable()
    {
        // Start flash countdown when enabled
        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
        }
        _flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private void OnDisable()
    {
        // Clean up coroutine if disabled externally
        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
            _flashCoroutine = null;
        }
    }

    private IEnumerator FlashRoutine()
    {
        float elapsed = 0f;

        while (elapsed < _flashDuration)
        {
            elapsed += Time.deltaTime;

            yield return null;
        }

        gameObject.SetActive(false);
        _flashCoroutine = null;
    }

    /// <summary>
    /// Trigger the flash effect. Simply enables the GameObject, which starts the flash countdown.
    /// </summary>
    public void Flash()
    {
        gameObject.SetActive(true);
    }
}