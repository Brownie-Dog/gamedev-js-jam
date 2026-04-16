using UnityEngine;
using UnityEngine.Assertions;

namespace Weapons
{
    [CreateAssetMenu(fileName = nameof(BaseWeaponSo), menuName = "ScriptableObjects/BaseWeapon")]
    public class BaseWeaponSo : ScriptableObject
    {
        [field: SerializeField] public Sprite Sprite { get; set; }

        // TODO: serialise weapon stats as SO
        [SerializeField] public int Damage = 1;
        
        [SerializeField] public float CooldownTime = 0f;
        
        protected void OnValidate()
        {
            Assert.IsNotNull(Sprite);
        }
    }
}