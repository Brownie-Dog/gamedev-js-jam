using UnityEngine;
using UnityEngine.Assertions;

public class PlayerDamageController : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerStatsSo _statsSo;
    [SerializeField] private KnockbackReceiver _knockbackReceiver;

    private void Awake()
    {
        Assert.IsNotNull(_statsSo);
        Assert.IsNotNull(_knockbackReceiver);
    }

    public void TakeDamage(DamageInfo info)
    {
        int newHealth = _statsSo.CurrentHealth - info.Damage;
        newHealth = Mathf.Max(newHealth, 0);

        _statsSo.UpdateHealth(newHealth);

        if (info.Knockback.sqrMagnitude > 0.001f)
        {
            _knockbackReceiver.Apply(info.Knockback);
        }

        if (newHealth <= 0)
        {
            _statsSo.Death();
        }
    }
}