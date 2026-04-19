using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.UI;
public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] 
    private GameObject _HeartPrefab;
    
    [SerializeField]
    private PlayerStatsSo _statsSo;
    
    private List<Image> _health = new List<Image>();

    void Awake()
    {
        Assert.IsNotNull(_HeartPrefab);
        Assert.IsNotNull(_statsSo);
    }
    
    private void Start()
    {
        _statsSo.CurrentHealth = _statsSo.MaxHealth;
        SetupHearts(_statsSo.MaxHealth);
        UpdateHealthUI(_statsSo.CurrentHealth);
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
            if (i < currentHealth)
            {
                _health[i].color = Color.white; 
            }
            else
            {
                _health[i].color = new Color(0.5f, 0.5f, 0.5f, 0.5f); 
            }
        }
    }
    
    private void OnEnable() 
    {
        _statsSo.OnHealthChanged += HandleHealthUpdate;
    }

    private void OnDisable() 
    {
        _statsSo.OnHealthChanged -= HandleHealthUpdate;
    }

    private void HandleHealthUpdate(object sender, EventArgs args)
    {
        UpdateHealthUI(_statsSo.CurrentHealth);
    }

}
