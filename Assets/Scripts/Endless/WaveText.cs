using UnityEngine;
using UnityEngine.UI; // Change to 'using TMPro;' if using TextMeshPro
using EndlessMode;
using TMPro;
using UnityEngine.Assertions;

public class EndlessUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text _waveText; // Change to 'private TMP_Text' for TextMeshPro
    [SerializeField] private GameObject _intermissionBanner; // Optional: A "Shop Open" banner
    [SerializeField] private float _proximityRange = 200f;

    private Transform _player;
    private EndlessModeMarker _marker;
    private bool _inRange;

    private void Awake()
    {
        Assert.IsNotNull(_waveText);

        _marker = FindObjectOfType<EndlessModeMarker>();
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        bool wasInRange = _inRange;
        _inRange = _marker != null && _player != null
                   && Vector2.Distance(_player.position, _marker.transform.position) <= _proximityRange;

        if (wasInRange && !_inRange)
        {
            _waveText.enabled = false;
            if (_intermissionBanner != null)
                _intermissionBanner.SetActive(false);
        }
        else if (!wasInRange && _inRange)
        {
            _waveText.enabled = true;
        }
    }

    private void OnEnable()
    {
        // Subscribe to the manager's events
        EndlessModeManager.OnWaveStarted += UpdateWaveDisplay;
        EndlessModeManager.OnIntermissionStarted += ShowIntermissionUI;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        EndlessModeManager.OnWaveStarted -= UpdateWaveDisplay;
        EndlessModeManager.OnIntermissionStarted -= ShowIntermissionUI;
    }

    private void UpdateWaveDisplay(int waveNumber)
    {
        if (!_inRange) return;

        _waveText.text = "WAVE: " + waveNumber;
        _waveText.enabled = true;

        if (_intermissionBanner != null)
            _intermissionBanner.SetActive(false);
    }

    private void ShowIntermissionUI()
    {
        if (!_inRange) return;

        _waveText.text = "Prepare yourself..";
        _waveText.enabled = true;

        if (_intermissionBanner != null)
            _intermissionBanner.SetActive(true);
    }
}