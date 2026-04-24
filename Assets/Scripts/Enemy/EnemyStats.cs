using ItemDrops;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyStats")]
public class EnemyStats : ScriptableObject, IWeaponData
{
    [Header("Movement")] 
    
    public float MovementSpeed = 5f;
    public float StoppingDistance = 0.5f;
    public float WanderSpeed = 2f;
    public float WanderDirectionChangeInterval = 2f;

    [Header("Detection")] 
    
    public float DetectionRadius = 7f;
    public float ChasingRadius = 15f;

    [Header("Combat")] 
    
    public float AttackCooldown = 1f;
    public float AttackRange = 1f;
    public int Damage = 1;
    public float KnockbackForce = 0f;

    [Header("Health")] 
    
    public int MaxHealth = 100;

    [Header("Drops")]

    public float DropChance = 0.05f;
    public ItemData GuaranteedItem;
    public float CoinDropChance = 0.5f;
    public int MinCoinDrop = 1;
    public int MaxCoinDrop = 3;

    int IWeaponData.Damage => Damage;
    float IWeaponData.KnockbackForce => KnockbackForce;
}