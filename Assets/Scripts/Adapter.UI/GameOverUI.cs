using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : UIElementBase
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Button _mainMenuButton;

    [SerializeField] private TextMeshProUGUI _collectedGoldText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _newHighscoreText;

    private GameStateService _gameStateService;

    [Header("Fade Settings")]
    [SerializeField] private float _fadeDuration = 1.0f;

    [Header("Stats Animation Settings")]
    [SerializeField] private float _goldCountDuration = 1.0f;
    [SerializeField] private float _scoreCountDuration = 1.2f;
    [SerializeField] private float _delayBetweenStats = 0.3f;
    [SerializeField] private AnimationCurve _countCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    [Header("Highscore Animation Settings")]
    [SerializeField] private float _highscorePopScale = 1.2f;
    [SerializeField] private float _highscorePopDuration = 0.5f;
    [SerializeField] private float _highscorePulseDuration = 0.8f;

    private int _targetGold;
    private int _targetScore;
    private bool _isNewHighscore;

    public void Initialize(GameStateService gameStateService)
    {
        _gameStateService = gameStateService;
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
    }
    
    private void OnMainMenuButtonClicked()
    {
        _gameStateService.TransitionTo(GameState.MainMenu);
    }

    public void SetStats(int collectedGold, int score, bool isNewHighscore)
    {
        _targetGold = collectedGold;
        _targetScore = score;
        _isNewHighscore = isNewHighscore;
    }

    public override void Show()
    {
        if (_canvasGroup != null)
        {
            // Hide highscore text initially
            if (_newHighscoreText != null)
            {
                _newHighscoreText.gameObject.SetActive(false);
            }

            // Reset text values
            _collectedGoldText.text = "0";
            _scoreText.text = "0";

            StartCoroutine(ShowSequence());
        }
    }

    public override void Hide()
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        if (_newHighscoreText != null)
        {
            _newHighscoreText.gameObject.SetActive(false);
        }
    }

    private IEnumerator ShowSequence()
    {
        // First fade in the panel
        yield return StartCoroutine(FadeIn());

        // Small delay after fade completes
        yield return new WaitForSecondsRealtime(0.2f);

        // Animate gold count
        yield return StartCoroutine(CountUpNumber(_collectedGoldText, 0, _targetGold, _goldCountDuration));

        // Delay between stats
        yield return new WaitForSecondsRealtime(_delayBetweenStats);

        // Animate score count
        yield return StartCoroutine(CountUpNumber(_scoreText, 0, _targetScore, _scoreCountDuration));

        // Show highscore if applicable
        if (_isNewHighscore && _newHighscoreText != null)
        {
            yield return new WaitForSecondsRealtime(0.2f);
            yield return StartCoroutine(AnimateHighscoreText());
        }
    }

    private IEnumerator FadeIn()
    {
        _canvasGroup.blocksRaycasts = true;

        float elapsed = 0f;
        while (elapsed < _fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / _fadeDuration);
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
    }

    private IEnumerator CountUpNumber(TextMeshProUGUI textField, int startValue, int endValue, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curveValue = _countCurve.Evaluate(t);
            
            int currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, curveValue));
            textField.text = currentValue.ToString();

            yield return null;
        }

        textField.text = endValue.ToString();
    }

    private IEnumerator AnimateHighscoreText()
    {
        _newHighscoreText.gameObject.SetActive(true);
        Transform textTransform = _newHighscoreText.transform;
        textTransform.localScale = Vector3.zero;

        // Pop in effect
        float elapsed = 0f;
        while (elapsed < _highscorePopDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / _highscorePopDuration);
            
            // Overshoot effect
            float scale = Mathf.Lerp(0f, _highscorePopScale, t);
            if (t > 0.7f)
            {
                float overshootT = (t - 0.7f) / 0.3f;
                scale = Mathf.Lerp(_highscorePopScale, 1f, overshootT);
            }
            
            textTransform.localScale = Vector3.one * scale;
            yield return null;
        }

        textTransform.localScale = Vector3.one;

        // Continuous pulse effect
        StartCoroutine(PulseHighscoreText(textTransform));
    }

    private IEnumerator PulseHighscoreText(Transform textTransform)
    {
        while (textTransform.gameObject.activeSelf)
        {
            float elapsed = 0f;
            
            // Scale up
            while (elapsed < _highscorePulseDuration / 2f)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / (_highscorePulseDuration / 2f));
                float scale = Mathf.Lerp(1f, 1.1f, t);
                textTransform.localScale = Vector3.one * scale;
                yield return null;
            }

            elapsed = 0f;
            
            // Scale down
            while (elapsed < _highscorePulseDuration / 2f)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / (_highscorePulseDuration / 2f));
                float scale = Mathf.Lerp(1.1f, 1f, t);
                textTransform.localScale = Vector3.one * scale;
                yield return null;
            }
        }
    }
}