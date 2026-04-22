using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using System.Collections;
using UnityEngine.Assertions;

public class VideoEffect : MonoBehaviour
{
    [SerializeField] private VideoPlayer _player;
    [SerializeField] private RawImage _display;

    private void Awake()
    {
        Assert.IsNotNull(_display);
        Assert.IsNotNull(_player);
    }
    
    public void Play(Action onComplete)
    {
        _display.gameObject.SetActive(true);
        _player.Play();
        
        StartCoroutine(WaitUntilFinished(onComplete));
    }

    private IEnumerator WaitUntilFinished(Action onComplete)
    {
        yield return new WaitForSecondsRealtime((float)_player.length);
        
        _player.Stop();
        _display.gameObject.SetActive(false);
        onComplete?.Invoke();
    }
}