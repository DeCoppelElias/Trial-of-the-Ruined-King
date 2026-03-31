using UnityEngine;

public class FloatingMovement : MonoBehaviour
{
    [Header("Float Settings")]
    [SerializeField] private bool _floatX = true;
    [SerializeField] private bool _floatY = true;
    [SerializeField] private bool _floatZ = true;

    [Header("Movement Parameters")]
    [SerializeField] private float _amplitude = 0.2f;
    [SerializeField] private float _frequency = 1f;
    [SerializeField] private Vector3 _randomOffset;

    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.localPosition;
        
        // Random offset to desynchronize multiple floating objects
        _randomOffset = new Vector3(
            Random.Range(0f, 2),
            Random.Range(0f, 2),
            Random.Range(0f, 2)
        );
    }

    private void Update()
    {
        float time = Time.time * _frequency;
        
        Vector3 offset = Vector3.zero;
        
        if (_floatX)
        {
            offset.x = Mathf.Sin(time + _randomOffset.x) * _amplitude;
        }
        
        if (_floatY)
        {
            offset.y = Mathf.Sin(time + _randomOffset.y) * _amplitude;
        }
        
        if (_floatZ)
        {
            offset.z = Mathf.Sin(time + _randomOffset.z) * _amplitude;
        }
        
        transform.localPosition = _startPosition + offset;
    }
}
