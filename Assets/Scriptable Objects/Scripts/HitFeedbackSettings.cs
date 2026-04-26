using UnityEngine;

// ScriptableObject to configure hit feedback effects (camera shake and vignette)
[CreateAssetMenu(fileName = "HitFeedbackSettings", menuName = "ScriptableObjects/Camera/HitFeedbackSettings")]
public class HitFeedbackSettings : ScriptableObject
{
    [Header("Shake")]
    public float shakeDuration = 0.15f;
    public float shakeMagnitude = 0.25f;

    [Header("Vignette")]
    public bool enableVignette = true;
    public float vignettePeakIntensity = 0.4f;
    public float vignetteRiseTime = 0.05f;
    public float vignetteFallTime = 0.2f;

    [Header("Paths")]
    public bool useCinemachineImpulseIfAvailable = true; // reserved for future CM integration
}
