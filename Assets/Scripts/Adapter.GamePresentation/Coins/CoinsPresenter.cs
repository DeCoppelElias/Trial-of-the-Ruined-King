using System.Collections;
using UnityEngine;

public class CoinsPresenter : MonoBehaviour
{
    [SerializeField] private GameObject _coinPrefab;
    [SerializeField] private AudioEvent _coinCollectSound;
    [SerializeField] private Transform _coinContainer;
    [SerializeField] private float _fadeInDuration = 1f;

    private GameObject _coinInstance;
    private IArena _arena;
    private AudioPlayer _audioPlayer;
    
    public void Initialize(IArena arena, AudioPlayer audioPlayer)
    {
        _arena = arena;
        _audioPlayer = audioPlayer;
    }

    public void HandleCoinCreated(Coin coin)
    {
        var worldPosition = _arena.GridToWorld(coin.Position);
        var rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        _coinInstance = Instantiate(_coinPrefab, worldPosition + new Vector3(0, 0.2f, 0), rotation, _coinContainer);
        _coinInstance.transform.localScale = new Vector3(Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f));

        StartCoroutine(FadeInCoin(_coinInstance));
    }

    public void HandleCoinRemoved(Coin coin)
    {
        if (_coinInstance != null)
        {
            _audioPlayer.Play(_coinCollectSound);
            Destroy(_coinInstance);
            _coinInstance = null;
        }
    }

    private IEnumerator FadeInCoin(GameObject coinObject)
    {
        Renderer[] renderers = coinObject.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            yield break;

        Material[] materials = new Material[renderers.Length];
        Color[] originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].material;
            originalColors[i] = materials[i].color;
            Color color = originalColors[i];
            color.a = 0f;
            materials[i].color = color;
        }

        float elapsedTime = 0f;

        while (elapsedTime < _fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / _fadeInDuration);

            for (int i = 0; i < materials.Length; i++)
            {
                Color color = originalColors[i];
                color.a = alpha;
                materials[i].color = color;
            }

            yield return null;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            Color color = originalColors[i];
            color.a = 1f;
            materials[i].color = color;
        }
    }
}