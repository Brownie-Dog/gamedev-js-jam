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
    
    private List<Image> _hearts = new List<Image>();

    private void Awake()
    {
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
            _hearts.Add(heart.GetComponent<Image>());
        }
    }

    public void UpdateHealthUI(int currentHealth)
    {
        for (int i = 0; i < _hearts.Count; i++)
        {
            if (i < currentHealth)
            {
                _hearts[i].color = Color.white; 
            }
            else
            {
                _hearts[i].color = new Color(0.5f, 0.5f, 0.5f, 0.5f); 
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
