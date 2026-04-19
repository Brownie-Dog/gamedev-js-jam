using System;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyDeath : MonoBehaviour
{
    [SerializeField] private EnemyHealth _health;

    private void Awake()
    {
        Assert.IsNotNull(_health);
    }

    private void OnEnable()
    {
        _health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        _health.OnDeath -= HandleDeath;
    }

    private void HandleDeath(object sender, EventArgs e)
    {
        Debug.Log($"{gameObject.name} has died.");
        Debug.Log($"Drops nothing hahaha");
    }
}