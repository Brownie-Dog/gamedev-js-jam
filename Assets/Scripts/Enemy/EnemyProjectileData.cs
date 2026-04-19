using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyProjectileData")]
public class EnemyProjectileData : ScriptableObject
{
    public float Speed = 10f;
    public float Lifetime = 5f;
    public float ImageRotation = 180f;
    public int Damage = 1;
    public int PoolDefaultCapacity = 15;
    public int PoolMaxSize = 30;
}
