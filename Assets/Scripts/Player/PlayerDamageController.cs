using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

// TODO: Split this up further
public class PlayerDamageController : MonoBehaviour, IDamageable
{
    [SerializeField]
    private PlayerStatsSo _statsSo;
    
    [SerializeField] 
    private PlayerHealthUI _healthUI;

    public void TakeDamage(int damage)
    {
        _statsSo.CurrentHealth -= damage;
        _statsSo.CurrentHealth = Mathf.Max(_statsSo.CurrentHealth, 0);
        
        if (_healthUI != null)
        {
            _healthUI.UpdateHealthUI(_statsSo.CurrentHealth);
        }
        
    }
    
}