using System;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyHealth _health;
    
    private void OnEnable()
    {
        if (_health != null) _health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        if (_health != null) _health.OnDeath -= HandleDeath;
    }
    
    private void HandleDeath(object sender, EventArgs e)
    {
        Debug.Log($"{gameObject.name} has died.");
        Debug.Log($"Drops nothing hahaha");
    }
}
