using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeTransition : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private float _fadeDuration = 0.3f;

    private Coroutine _activeFade;

    private void Awake()
    {
        SetAlpha(0f);
    }

    public void Play(Action onBlack)
    {
        if (_activeFade != null)
        {
            StopCoroutine(_activeFade);
        }

        _activeFade = StartCoroutine(RunFade(onBlack));
    }

    private IEnumerator RunFade(Action onBlack)
    {
        yield return Fade(0f, 1f, _fadeDuration);

        onBlack?.Invoke();

        yield return Fade(1f, 0f, _fadeDuration);

        _activeFade = null;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            SetAlpha(Mathf.Lerp(from, to, t));
            yield return null;
        }

        SetAlpha(to);
    }

    private void SetAlpha(float alpha)
    {
        var color = _image.color;
        color.a = alpha;
        _image.color = color;
    }
}
