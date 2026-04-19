using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/PlayerStats")]
public class PlayerStatsSo : ScriptableObject
{
    public float MovementSpeed = 5f;
    public int MaxHealth = 10;
    public int CurrentHealth = 10;
        
    public EventHandler OnHealthChanged; 
    public EventHandler OnPlayerDeath;

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
    
}
