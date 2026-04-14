using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class IntroSequenceController : MonoBehaviour
{
    [SerializeField]
    private Animation _introAnimation;

    [SerializeField]
    private AudioSource _introAudio;

    private bool _hasLoadedNextScene = false;

    private void Awake()
    {
        Assert.IsNotNull(_introAnimation);
        Assert.IsNotNull(_introAudio);
    }

    private void OnEnable()
    {
        PlayIntroAnimation();
    }

    private void Update()
    {
        if (_hasLoadedNextScene)
        {
            return;
        }

        if (!_introAnimation.isPlaying)
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        _hasLoadedNextScene = true;

        // Stop audio if it's still playing during the transition
        if (_introAudio.isPlaying)
        {
            _introAudio.Stop();
        }

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogError("No more scenes in Build Settings to load!");
        }
    }

    private void PlayIntroAnimation()
    {
        _introAnimation.Rewind();
        _introAnimation.Play();
    }

    private void StopIntroAnimation()
    {
        _introAnimation.Rewind();
        _introAnimation.Stop();
    }

    private void AnimationEvent_OnPlayIntroSoundEvent()
    {
        _introAudio.Play();
    }
}
