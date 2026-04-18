using UnityEngine;

public class MeleeAttack : EnemyAttack
{
    protected override void Attack()
    {
        Debug.Log("MELEE ATTACK");
        
        // do damage
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ExecuteAttack();
        }
    }
}
