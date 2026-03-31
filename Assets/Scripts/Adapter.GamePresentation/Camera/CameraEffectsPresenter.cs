using UnityEngine;
public class CameraEffectsPresenter : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private Vector3 _originalPosition;
    private float _shakeDuration;
    private float _shakeMagnitude;
    private float _shakeElapsed;

    private float _originalFieldOfView;
    private Quaternion _originalRotation;
    private float _zoomInDuration;
    private float _zoomOutDuration;
    private float _zoomAmount;
    private float _rotationAmount;
    private float _zoomElapsed;
    private bool _isZooming;
    private bool _isZoomingIn;

    private void Start()
    {
        _camera = Camera.main;
        if (_camera != null)
        {
            _originalFieldOfView = _camera.fieldOfView;
            _originalRotation = _camera.transform.localRotation;
        }
    }
    public void ShakeCamera(float duration, float magnitude)
    {
        if (_camera == null) return;

        _originalPosition = _camera.transform.localPosition;
        _shakeDuration = duration;
        _shakeMagnitude = magnitude;
        _shakeElapsed = 0f;
    }

    public void ZoomAndRotateCamera(float zoomInDuration = 2f, float zoomOutDuration = 0.5f, float zoomAmount = 10f, float rotationAmount = 5f)
    {
        if (_camera == null) return;

        _originalFieldOfView = _camera.fieldOfView;
        _originalRotation = _camera.transform.localRotation;
        _zoomInDuration = zoomInDuration;
        _zoomOutDuration = zoomOutDuration;
        _zoomAmount = zoomAmount;
        _rotationAmount = rotationAmount;
        _zoomElapsed = 0f;
        _isZooming = true;
        _isZoomingIn = true;
    }
    private void Update()
    {
        if (_camera == null) return;

        if (_shakeElapsed < _shakeDuration)
        {
            _shakeElapsed += Time.deltaTime;
            float x = Random.Range(-1f, 1f) * _shakeMagnitude;
            float y = Random.Range(-1f, 1f) * _shakeMagnitude;
            _camera.transform.localPosition = _originalPosition + new Vector3(x, y, 0f);

            if (_shakeElapsed >= _shakeDuration)
                _camera.transform.localPosition = _originalPosition;
        }

        if (_isZooming)
        {
            _zoomElapsed += Time.deltaTime;

            if (_isZoomingIn)
            {
                float currentDuration = _zoomInDuration;
                float t = Mathf.Clamp01(_zoomElapsed / currentDuration);
                t = Mathf.SmoothStep(0f, 1f, t);

                _camera.fieldOfView = Mathf.Lerp(_originalFieldOfView, _originalFieldOfView - _zoomAmount, t);
                _camera.transform.localRotation = Quaternion.Lerp(_originalRotation, _originalRotation * Quaternion.Euler(0f, 0f, _rotationAmount), t);

                if (_zoomElapsed >= currentDuration)
                {
                    _isZoomingIn = false;
                    _zoomElapsed = 0f;
                }
            }
            else
            {
                float currentDuration = _zoomOutDuration;
                float t = Mathf.Clamp01(_zoomElapsed / currentDuration);
                t = Mathf.SmoothStep(0f, 1f, t);

                _camera.fieldOfView = Mathf.Lerp(_originalFieldOfView - _zoomAmount, _originalFieldOfView, t);
                _camera.transform.localRotation = Quaternion.Lerp(_originalRotation * Quaternion.Euler(0f, 0f, _rotationAmount), _originalRotation, t);

                if (_zoomElapsed >= currentDuration)
                {
                    _camera.fieldOfView = _originalFieldOfView;
                    _camera.transform.localRotation = _originalRotation;
                    _isZooming = false;
                }
            }
        }
    }
}