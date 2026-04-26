using System.Collections;
using UnityEngine;
using CameraFeedback;

// Lightweight camera shake fallback (no Cinemachine required)
public class CameraShakeFallback : MonoBehaviour, ICameraShake
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _defaultShakeDuration = 0.15f;
    [SerializeField] private float _defaultShakeMagnitude = 0.25f;

    private Vector3 _originalLocalPosition;
    private Coroutine _shakeRoutine;

    private void Awake()
    {
        if (_cameraTransform == null)
        {
            if (Camera.main != null)
                _cameraTransform = Camera.main.transform;
        }
        AssertNotNull(_cameraTransform, nameof(_cameraTransform));
        _originalLocalPosition = _cameraTransform.localPosition;
    }

    public void TriggerShake(float duration, float magnitude)
    {
        // Use provided duration/magnitude or fall back to defaults
        if (duration <= 0f) duration = _defaultShakeDuration;
        if (magnitude <= 0f) magnitude = _defaultShakeMagnitude;

        if (_shakeRoutine != null) StopCoroutine(_shakeRoutine);
        _shakeRoutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float damper = 1f - Mathf.Clamp01((elapsed / duration) * 2f); // quick ease out
            Vector3 offset = Random.insideUnitSphere * magnitude * damper;
            offset.z = 0f;
            _cameraTransform.localPosition = _originalLocalPosition + offset;
            elapsed += Time.deltaTime;
            yield return null;
        }
        _cameraTransform.localPosition = _originalLocalPosition;
        _shakeRoutine = null;
    }

    private void AssertNotNull(Object obj, string name)
    {
        if (obj == null) Debug.LogError(name + " is null on CameraShakeFallback; ensure a camera transform is assigned.");
    }
}
