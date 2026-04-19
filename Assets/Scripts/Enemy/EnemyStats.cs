using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [Header("Movement")]
    public float MovementSpeed = 5f;
    public float StoppingDistance = 0.5f;

    [Header("Detection")]
    public float DetectionRadius = 7f;
    public float ChasingRadius = 15f;

    [Header("Combat")]
    public float AttackCooldown = 1f;
    public float AttackRange = 1f;
    public int Damage = 1;

    [Header("Health")]
    public int MaxHealth = 100;
}