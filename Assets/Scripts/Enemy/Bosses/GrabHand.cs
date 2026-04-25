using System;
using UnityEngine;

public class GrabHand : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHealth = 3;

    private int _currentHealth;
    private bool _isActive;
    private int _playerLayer;

    public bool IsPlayerInReach { get; private set; }
    public event EventHandler OnGrabBroken;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _playerLayer = LayerMask.NameToLayer(GlobalConstants.PLAYER_LAYER);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isActive && other.gameObject.layer == _playerLayer)
        {
            IsPlayerInReach = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (_isActive && other.gameObject.layer == _playerLayer)
        {
            IsPlayerInReach = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == _playerLayer)
        {
            IsPlayerInReach = false;
        }
    }

    public void Activate()
    {
        _currentHealth = _maxHealth;
        _isActive = true;
        IsPlayerInReach = false;
    }

    public void Deactivate()
    {
        _isActive = false;
        IsPlayerInReach = false;
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
