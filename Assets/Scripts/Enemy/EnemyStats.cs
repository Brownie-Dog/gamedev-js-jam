using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [Header("Movement")]
    public float movementSpeed = 5f;
    public float stoppingDistance = 0.5f;

    [Header("Detection")]
    public float detectionRadius = 7f;
    public float chasingRadius = 15f;

    [Header("Combat")]
    public float attackCooldown = 1f;
    public float attackRange = 1f;

    [Header("Health")]
    public int maxHealth = 100;
}