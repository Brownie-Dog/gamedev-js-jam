using System;
using UnityEngine;
using UnityEngine.Assertions;

public class SoundEffect : MonoBehaviour
{
    [SerializeField] private float _startTime = 0f;
    [SerializeField, Range(0, 1)] private float _volume = 1f;
    [SerializeField] private AudioSource _audioSource;

    private void Awake()
    {
        Assert.IsNotNull(_audioSource);
        Assert.IsNotNull(_audioSource.clip);
    }

    public void Play()
    {
        _audioSource.volume = _volume;
        _audioSource.Play();
        _audioSource.time = _startTime;
    }
    
    public void Stop()
    {
        _audioSource.Stop();
    }

    public bool IsPlaying()
    {
        return _audioSource.isPlaying;
    }
}
