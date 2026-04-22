using UnityEngine;

public struct DamageInfo
{
    public int Damage;
    public Vector2 Knockback;

    public DamageInfo(int damage, Vector2 knockback)
    {
        Damage = damage;
        Knockback = knockback;
    }
}