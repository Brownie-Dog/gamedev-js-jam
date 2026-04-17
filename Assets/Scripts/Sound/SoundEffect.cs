using System;
using UnityEngine;
public class SoundEffect : MonoBehaviour
{
    [SerializeField] private float _startTime = 0f;
    [SerializeField] [Range(0, 1)] private float _volume = 1f;
    [SerializeField] private AudioSource _audioSource;

    public void Play()
    {
        if (!_audioSource.clip || !_audioSource) return;
        _audioSource.volume = _volume;
        _audioSource.Play();
        _audioSource.time = _startTime;
    }
    
    public void Stop()
    {
        if (_audioSource)
        {
            _audioSource.Stop();
        }
    }

    public bool IsPlaying => _audioSource && _audioSource.isPlaying;
}
