using UnityEngine;

public struct DamageInfo
{
    public int Damage;
    public Vector2 Knockback;
    public float KnockbackDuration;

    public DamageInfo(int damage, Vector2 knockback, float knockbackDuration = -1f)
    {
        Damage = damage;
        Knockback = knockback;
        KnockbackDuration = knockbackDuration;
    }
}