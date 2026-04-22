using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
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
        Assert.IsNotNull(_HeartPrefab);
    }
    
    private void Start()
    {
        SetupHearts(_statsSo.MaxHealth);
        UpdateHealthUI(_statsSo.CurrentHealth);
    }

    private void SetupHearts(int maxHealth)
    {
        for (int i = 0; i < maxHealth; i++)
        {
            if (i < _hearts.Count)
            {
                _hearts[i].gameObject.SetActive(true);
            }
            else
            {
                GameObject heart = Instantiate(_HeartPrefab, transform);
                _hearts.Add(heart.GetComponent<Image>());
            }
        }

        for (int i = maxHealth; i < _hearts.Count; i++)
        {
            _hearts[i].gameObject.SetActive(false);
        }
    }

    public void UpdateHealthUI(int currentHealth)
    {
        SetupHearts(_statsSo.MaxHealth);

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
