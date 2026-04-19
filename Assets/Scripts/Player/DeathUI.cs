using UnityEngine;
using UnityEngine.UI; // Needed for RawImage
using UnityEngine.Video; // Needed for VideoPlayer
using System.Collections;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class DeathUI : MonoBehaviour
{
    [SerializeField] private PlayerStatsSo _statsSo;
    [SerializeField] private GameObject _canvasObject;
    [SerializeField] private GameObject _bsodLayer;
    [SerializeField] private GameObject _staticLayer;

    // The new video components
    [SerializeField] private RawImage _videoDisplay; 
    [SerializeField] private VideoPlayer _videoPlayer;

    private void Awake()
    {
        Assert.IsNotNull(_statsSo);
        Assert.IsNotNull(_staticLayer);
        Assert.IsNotNull(_canvasObject);
        Assert.IsNotNull(_bsodLayer);
        Assert.IsNotNull(_videoPlayer);
        Assert.IsNotNull(_videoDisplay);
    }
    private void Start()
    {
        _canvasObject.SetActive(false);
        _videoDisplay.gameObject.SetActive(false);
        _bsodLayer.SetActive(false);
    }

    private void OnEnable() => _statsSo.OnPlayerDeath += StartSequence;
    private void OnDisable() => _statsSo.OnPlayerDeath -= StartSequence;

    private void StartSequence(object sender, System.EventArgs args)
    {
        _canvasObject.SetActive(true);
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        
        _staticLayer.SetActive(true);
        yield return new WaitForSeconds(0.4f); 
        _staticLayer.SetActive(false);
        
        _videoDisplay.gameObject.SetActive(true);
        _videoPlayer.Play();

        yield return new WaitForSeconds((float)_videoPlayer.length);

        _bsodLayer.SetActive(true);
        _videoDisplay.gameObject.SetActive(false);

        // Move this to level manager or something
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}