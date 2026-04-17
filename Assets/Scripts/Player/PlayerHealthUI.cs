using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] 
    private GameObject _HeartPrefab;
    
    [SerializeField]
    private PlayerStatsSo _statsSo;
    
    private List<Image> _health = new List<Image>();

    private void Start()
    {
        SetupHearts(_statsSo.MaxHealth);
        _statsSo.CurrentHealth = _statsSo.MaxHealth;
    }
    
    private void SetupHearts(int maxHealth)
    {
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(_HeartPrefab, transform);
            _health.Add(heart.GetComponent<Image>());
        }
    }

    public void UpdateHealthUI(int currentHealth)
    {
        for (int i = 0; i < _health.Count; i++)
        {
            _health[i].enabled = i < currentHealth;
        }
    }

}
