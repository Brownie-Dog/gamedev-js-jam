using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteController : MonoBehaviour
{
    [SerializeField] private Volume _volume;
    private Vignette _vignette;
    private Coroutine _pulse;

    private void Awake()
    {
        Assert.IsNotNull(_volume);
        if (_volume != null && _volume.profile != null)
        {
            _volume.profile.TryGet<Vignette>(out _vignette);
        }
        Assert.IsNotNull(_vignette);
    }

    public void TriggerHit(float peakIntensity, float riseTime, float fallTime)
    {
        if (_vignette == null) return;
        if (_pulse != null) StopCoroutine(_pulse);
        _pulse = StartCoroutine(Animate(peakIntensity, riseTime, fallTime));
    }

    private IEnumerator Animate(float peak, float rise, float fall)
    {
        // Rise
        float t = 0f;
        while (t < rise)
        {
            _vignette.intensity.value = Mathf.Lerp(0f, peak, t / rise);
            t += Time.deltaTime;
            yield return null;
        }
        _vignette.intensity.value = peak;
        yield return new WaitForSeconds(0.05f);
        t = 0f;
        while (t < fall)
        {
            _vignette.intensity.value = Mathf.Lerp(peak, 0f, t / fall);
            t += Time.deltaTime;
            yield return null;
        }
        _vignette.intensity.value = 0f;
        _pulse = null;
    }
}
