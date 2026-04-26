using UnityEngine;
using UnityEngine.UI; // Change to 'using TMPro;' if using TextMeshPro
using EndlessMode;
using TMPro;
using UnityEngine.Assertions;

public class EndlessUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text _waveText; // Change to 'private TMP_Text' for TextMeshPro
    [SerializeField] private GameObject _intermissionBanner; // Optional: A "Shop Open" banner

    private void Awake()
    {
        Assert.IsNotNull(_waveText);
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
        _waveText.text = "WAVE: " + waveNumber;
        
        if (_intermissionBanner != null)
            _intermissionBanner.SetActive(false);
    }

    private void ShowIntermissionUI()
    {
        _waveText.text = "Prepare yourself..";
        
        if (_intermissionBanner != null)
            _intermissionBanner.SetActive(true);
    }
}