using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private EnemyStats _stats;
    private int _currentHealth;

    public EventHandler OnDeath;
    public EventHandler OnHealthChange; 

    private void Awake()
    {
        _currentHealth = _stats.maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        
        // red sprite flashin sound effect
        OnHealthChange?.Invoke(this, EventArgs.Empty);

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
    public int GetCurrentHealth() => _currentHealth;
    
}
