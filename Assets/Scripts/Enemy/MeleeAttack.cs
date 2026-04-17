using UnityEngine;

public class MeleeAttack : EnemyAttack
{
    [Header("Sound Effect")]
    [SerializeField] private SoundEffect _attackSound;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && CanAttack())
        {
            Debug.Log("MELEE ATTACK");
            _attackSound.Play();
            ResetCooldown();
        }
    }
}
