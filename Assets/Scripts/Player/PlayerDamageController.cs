using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

// TODO: Split this up further
public class PlayerDamageController : MonoBehaviour, IDamageable
{
    [SerializeField]
    private PlayerStats _stats;

    [SerializeField]
    private int _currentHealth = 0;

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
    }
    
    private void Awake()
    {
        _currentHealth = _stats.MaxHealth;
    }
    
}