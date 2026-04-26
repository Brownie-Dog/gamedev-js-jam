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
    private static Vector3 _pendingEndlessRespawnPosition;

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
            _pendingEndlessRespawnPosition = respawnPoint.position;
            SceneManager.sceneLoaded += OnSceneLoadedForEndlessRespawn;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        // Reset stage: respawn all enemies and reset boss rooms
        StageManager.Instance?.ResetStage();

        if (respawnPoint != null)
        {
            var player = GameObject.FindGameObjectWithTag(GlobalConstants.PLAYER_TAG);
            if (player != null)
            {
                player.transform.position = respawnPoint.position;
            }
        }

        _canvasObject.SetActive(false);
        _bsodLayer.SetActive(false);
    }

    private static void OnSceneLoadedForEndlessRespawn(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoadedForEndlessRespawn;

        var player = GameObject.FindGameObjectWithTag(GlobalConstants.PLAYER_TAG);
        if (player != null)
        {
            player.transform.position = _pendingEndlessRespawnPosition;
        }

        // Refresh the respawn point reference to the new scene's endless marker
        var marker = GameObject.FindObjectOfType<EndlessModeMarker>();
        if (marker != null)
        {
            CurrentRespawnPoint = marker.transform;
        }
    }
}