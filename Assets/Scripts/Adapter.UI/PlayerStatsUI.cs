using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerStatsUI : UIElementBase
{
    private bool _initialized = false;
    private TMP_Text _text;
    private int _currentValue;

    [Header("Animation Settings")]
    [SerializeField] private bool _enableAnimation = true;
    [SerializeField] private float _punchScaleAmount = 1.3f;
    [SerializeField] private float _punchDuration = 0.3f;
    [SerializeField] private AnimationCurve _punchCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Coroutine _currentAnimation;

    public override void Hide()
    {
        if (!_initialized) return;
        this.gameObject.SetActive(false);
    }

    public override void Show()
    {
        if (!_initialized) return;
        this.gameObject.SetActive(true);
    }

    public void Initialize(int initial)
    {
        _initialized = true;
        _text = GetComponentInChildren<TMP_Text>();
        _currentValue = initial;
        _text.text = initial.ToString();
    }

    public void HandleUpdate(int newValue)
    {
        if (newValue > _currentValue && _enableAnimation)
        {
            if (_currentAnimation != null)
            {
                StopCoroutine(_currentAnimation);
            }
            _currentAnimation = StartCoroutine(AnimateValueIncrease(newValue));
        }
        else
        {
            _currentValue = newValue;
            _text.text = newValue.ToString();
        }
    }

    private IEnumerator AnimateValueIncrease(int newValue)
    {
        _text.text = newValue.ToString();
        _currentValue = newValue;

        Transform textTransform = _text.transform;
        Vector3 originalScale = Vector3.one;

        float elapsed = 0f;

        // Scale up
        while (elapsed < _punchDuration / 2f)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / (_punchDuration / 2f));
            float curveValue = _punchCurve.Evaluate(t);
            float scale = Mathf.Lerp(1f, _punchScaleAmount, curveValue);
            textTransform.localScale = originalScale * scale;
            yield return null;
        }

        elapsed = 0f;

        // Scale down
        while (elapsed < _punchDuration / 2f)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / (_punchDuration / 2f));
            float curveValue = _punchCurve.Evaluate(t);
            float scale = Mathf.Lerp(_punchScaleAmount, 1f, curveValue);
            textTransform.localScale = originalScale * scale;
            yield return null;
        }

        textTransform.localScale = originalScale;
        _currentAnimation = null;
    }
}
