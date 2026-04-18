using System;
using UnityEngine;

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

    public EventHandler OnHealthChanged;

    private void OnEnable()
    {
        MovementSpeed = _initialMovementSpeed;
        MaxHealth = _initialMaxHealth;
        CurrentHealth = MaxHealth;
    }

    public void UpdateHealth(int newHealth)
    {
        CurrentHealth = newHealth;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }
}
