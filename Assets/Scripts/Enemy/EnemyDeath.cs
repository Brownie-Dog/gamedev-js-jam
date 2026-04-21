using System;
using ItemDrops;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class EnemyDeath : MonoBehaviour
{
    [SerializeField]
    private EnemyHealth _health;

    [SerializeField]
    private EnemyStats _stats;

    private void Awake()
    {
        Assert.IsNotNull(_health);
        Assert.IsNotNull(_stats);
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
        if (Random.value > _stats.DropChance)
        {
            return;
        }

        ItemDropManager.Instance.SpawnItemDropObject(transform.position, _stats.GuaranteedItem);
    }
}