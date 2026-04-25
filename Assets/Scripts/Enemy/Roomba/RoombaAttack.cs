using UnityEngine;
using UnityEngine.Assertions;

public class RoombaAttack : MeleeEnemyAttack
{
    protected override void Attack()
    {
        base.Attack();
        Suicide();
    }

    private void Suicide()
    {
        const float delay = 0.5f;
        Destroy(gameObject, delay);
    }
}