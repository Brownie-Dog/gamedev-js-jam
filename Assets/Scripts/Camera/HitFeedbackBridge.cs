using System;
using UnityEngine;
using UnityEngine.Assertions;
using CameraFeedback;

// Bridge that listens to health changes and triggers camera shake and vignette feedback
public class HitFeedbackBridge : MonoBehaviour
{
    [SerializeField] private PlayerStatsSo _statsSo;
    [SerializeField] private HitFeedbackSettings _settings;
    [SerializeField] private CameraShakeFallback _cameraShake;
    [SerializeField] private VignetteController _vignette;

    private int _lastHealth;

    private void Awake()
    {
        Assert.IsNotNull(_statsSo);
        Assert.IsNotNull(_settings);
        Assert.IsNotNull(_cameraShake);
        Assert.IsNotNull(_vignette);
        _lastHealth = _statsSo.CurrentHealth;
    }

    private void OnEnable()
    {
        _statsSo.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        _statsSo.OnHealthChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(object sender, EventArgs args)
    {
        int current = _statsSo.CurrentHealth;
        if (current < _lastHealth) // took damage
        {
            int damage = _lastHealth - current;
            float magnitude = _settings.shakeMagnitude * (1f + (float)damage / Mathf.Max(1, _lastHealth));
            _cameraShake.TriggerShake(_settings.shakeDuration, magnitude);
            if (_settings.enableVignette)
            {
                _vignette.TriggerHit(_settings.vignettePeakIntensity, _settings.vignetteRiseTime, _settings.vignetteFallTime);
            }
        }
        _lastHealth = current;
    }
}
