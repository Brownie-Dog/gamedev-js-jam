using UnityEngine;

[System.Serializable]
public class SoundEffect
{
    [SerializeField] 
    private AudioSource _source;
    [SerializeField]
    private AudioClip _audioClip;
    [SerializeField]
    private float _startTime = 0f;
    [SerializeField]
    [Range(0, 1)] private float _volume = 1f;

    public void Play(AudioSource source)
    {
        if (!_audioClip|| !source) return;
        source.clip = _audioClip;
        source.volume = _volume;
        source.Play();
        source.time = _startTime;
    }
    
    public bool IsPlaying(AudioSource source) => source && source.isPlaying;
}
