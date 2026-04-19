using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private EnemyStats _stats;
    [SerializeField] private int _currentHealth;

    public EventHandler OnDeath;
    public EventHandler OnHealthChanged; 

    private void Awake()
    {
        Assert.NotNull(_stats);
        _currentHealth = _stats.maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        
        // red sprite flashing, sound effect
        OnHealthChanged?.Invoke(this, EventArgs.Empty);

        if (IsDead())
        {
            // drop item or something 
            OnDeath?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);
        }
    }
    
    private bool IsDead()
    {
        return _currentHealth <= 0;
    }
}
