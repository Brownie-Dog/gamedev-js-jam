using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/PlayerStats")]
public class PlayerStatsSo : ScriptableObject
{
    [SerializeField]
    private float _initialMovementSpeed = 5f;

    [SerializeField]
    private int _initialMaxHealth = 10;

    public float MovementSpeed;
    public int MaxHealth;
    public int CurrentHealth;
    public int Currency;

    public event Action<int> OnCurrencyChanged;
    public EventHandler OnHealthChanged;
    public EventHandler OnPlayerDeath;

    private void OnEnable()
    {
        MovementSpeed = _initialMovementSpeed;
        MaxHealth = _initialMaxHealth;
        CurrentHealth = MaxHealth;
        Currency = 0;
    }

    public void UpdateHealth(int newHealth)
    {
        CurrentHealth = newHealth;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Death()
    {
        CurrentHealth = MaxHealth;
        UpdateHealth(CurrentHealth);
        OnPlayerDeath?.Invoke(this, EventArgs.Empty);
    }

    public void AddCurrency(int amount)
    {
        Currency += amount;
        OnCurrencyChanged?.Invoke(Currency);
    }

    public bool TrySpendCurrency(int amount)
    {
        if (Currency < amount)
        {
            return false;
        }

        Currency -= amount;
        OnCurrencyChanged?.Invoke(Currency);
        return true;
    }
}