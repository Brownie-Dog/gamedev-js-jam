using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

// TODO: Split this up further
public class PlayerDamageController : MonoBehaviour, IDamageable
{
    [SerializeField]
    private PlayerStatsSo _statsSo;

    public void TakeDamage(int damage)
    {
        int newHealth = _statsSo.CurrentHealth - damage;
        newHealth = Mathf.Max(newHealth, 0);
        
        _statsSo.UpdateHealth(newHealth);
        
    }
    private void Awake()
    {
        Assert.IsNotNull(_statsSo);
    }
    
}
