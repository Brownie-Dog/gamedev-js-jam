using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.Assertions;

[RequireComponent(typeof(RawImage))]
public class StaticEffect : MonoBehaviour
{
    private RawImage _rawImage;

    private void Awake()
    {
        _rawImage = GetComponent<RawImage>();
        Assert.IsNotNull(_rawImage);
    }

    private void Update()
    {
        // Flicker logic is self-contained here
        _rawImage.uvRect = new Rect(UnityEngine.Random.value, UnityEngine.Random.value, 1, 1);
        
        // Force refresh while Time.timeScale = 0
        _rawImage.SetVerticesDirty();
    }

    public void Play(float duration, Action onComplete)
    {
        gameObject.SetActive(true);
        StartCoroutine(RunSequence(duration, onComplete));
    }

    private IEnumerator RunSequence(float duration, Action onComplete)
    {
        yield return new WaitForSecondsRealtime(duration);
        gameObject.SetActive(false);
        onComplete?.Invoke();
    }
}