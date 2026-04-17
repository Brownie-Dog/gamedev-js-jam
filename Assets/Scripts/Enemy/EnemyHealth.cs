using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;

    public EventHandler OnDeath;
    public EventHandler OnHealthChange; 

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        OnHealthChange?.Invoke(this, EventArgs.Empty);

        if (IsDead())
        {
            OnDeath?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);
        }
    }
    
    private bool IsDead()
    {
        return _currentHealth <= 0;
    }
    public int GetCurrentHealth() => _currentHealth;
    
}
