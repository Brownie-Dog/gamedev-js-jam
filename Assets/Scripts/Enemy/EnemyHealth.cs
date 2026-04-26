using System;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyStats _stats;
    [SerializeField] private int _currentHealth;
    [SerializeField] private KnockbackReceiver _knockbackReceiver;

    public EventHandler OnDeath;
    public EventHandler OnHealthChanged;

    public float HealthPercent => (float)_currentHealth / _stats.MaxHealth;

    private void Awake()
    {
        Assert.IsNotNull(_stats);
        Assert.IsNotNull(_knockbackReceiver);
        _currentHealth = _stats.MaxHealth;
    }

    private void OnEnable()
    {
        if (_stats != null)
            _currentHealth = _stats.MaxHealth;
    }

    public void TakeDamage(DamageInfo info)
    {
        _currentHealth -= info.Damage;

        if (info.Knockback.sqrMagnitude > 0.001f)
        {
            _knockbackReceiver.Apply(info.Knockback);
        }

        OnHealthChanged?.Invoke(this, EventArgs.Empty);

        if (IsDead())
        {
            OnDeath?.Invoke(this, EventArgs.Empty);
            gameObject.SetActive(false);
        }
    }

    private bool IsDead()
    {
        return _currentHealth <= 0;
    }
}