using System;
using UnityEngine;

public class GrabHand : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHealth = 3;

    private int _currentHealth;
    private bool _isActive;

    public event EventHandler OnGrabBroken;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void Activate()
    {
        _currentHealth = _maxHealth;
        _isActive = true;
    }

    public void Deactivate()
    {
        _isActive = false;
    }

    public void TakeDamage(DamageInfo info)
    {
        if (!_isActive)
        {
            return;
        }

        _currentHealth -= info.Damage;

        if (_currentHealth <= 0)
        {
            _isActive = false;
            OnGrabBroken?.Invoke(this, EventArgs.Empty);
        }
    }
}