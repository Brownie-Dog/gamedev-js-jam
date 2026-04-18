using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DeathUI : MonoBehaviour
{
    [SerializeField] private PlayerStatsSo _statsSo;
    
    // DRAG THE 'CANVAS' CHILD INTO THIS SLOT IN THE INSPECTOR
    [SerializeField] private GameObject _canvasObject; 

    [SerializeField] private GameObject _staticLayer;
    [SerializeField] private GameObject _bsodLayer;
    [SerializeField] private Animator _screenAnimator;
    private void Start()
    {
        _canvasObject.SetActive(false);
    
        _staticLayer.SetActive(false);
        _bsodLayer.SetActive(false);
    }
    
    private void OnEnable() => _statsSo.OnPlayerDeath += StartSequence;
    private void OnDisable() => _statsSo.OnPlayerDeath -= StartSequence;

    private void StartSequence(object sender, EventArgs args)
    {
        _canvasObject.SetActive(true); 
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        // _staticLayer.SetActive(true);
        // yield return new WaitForSeconds(1f);
        
        _staticLayer.SetActive(false);
        _screenAnimator.SetTrigger("TurnOff");
        
        yield return new WaitForSeconds(0.5f);
        _bsodLayer.SetActive(true);
        
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}