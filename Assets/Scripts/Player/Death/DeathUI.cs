using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class DeathUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _staticDuration = 0.4f;
    [SerializeField] private float _bsodDuration = 2f;

    [Header("Dependencies")]
    [SerializeField] private PlayerStatsSo _statsSo;
    [SerializeField] private GameObject _canvasObject;
    [SerializeField] private GameObject _bsodLayer;
    [SerializeField] private StaticEffect _staticEffect;
    [SerializeField] private VideoEffect _videoEffect;
    [SerializeField] private Transform _defaultRespawnPoint;

    public static Transform CurrentRespawnPoint { get; set; }

    private void Awake()
    {
        Assert.IsNotNull(_statsSo);
        Assert.IsNotNull(_canvasObject);
        Assert.IsNotNull(_bsodLayer);
        Assert.IsNotNull(_staticEffect);
        Assert.IsNotNull(_videoEffect);
    }

    private void OnEnable() => _statsSo.OnPlayerDeath += StartSequence;
    private void OnDisable() => _statsSo.OnPlayerDeath -= StartSequence;

    private void StartSequence(object sender, EventArgs args)
    {
        Time.timeScale = 0f;
        _canvasObject.SetActive(true);

        _staticEffect.Play(_staticDuration, onComplete: () => 
        {
            _videoEffect.Play(onComplete: () => 
            {
                StartCoroutine(FinalBSODSequence());
            });
        });
    }

    private IEnumerator FinalBSODSequence()
    {
        _bsodLayer.SetActive(true);
        yield return new WaitForSecondsRealtime(_bsodDuration);
        
        RestartLevel();
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;

        var respawnPoint = CurrentRespawnPoint ?? _defaultRespawnPoint;

        if (respawnPoint != null && respawnPoint.GetComponent<EndlessModeMarker>() != null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        if (respawnPoint != null)
        {
            var player = GameObject.FindGameObjectWithTag(GlobalConstants.PLAYER_TAG);
            if (player != null)
            {
                player.transform.position = respawnPoint.position;
            }
        }
    }
}