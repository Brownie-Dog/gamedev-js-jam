using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField]
    private int _damageAmount = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable target = collision.gameObject.GetComponent<IDamageable>();
        target?.TakeDamage(_damageAmount);
    }
    
}
