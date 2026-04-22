using UnityEngine;
using UnityEngine.Assertions;

namespace Weapons
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class WeaponAutoFireController : MonoBehaviour
    {
        [SerializeField] private LayerMask _enemyLayerMask;

        private Weapon _weapon;
        private int _enemiesInRange;

        private void Awake()
        {
            _weapon = GetComponentInParent<Weapon>();
            Assert.IsNotNull(_weapon);

            GetComponent<CircleCollider2D>().isTrigger = true;
        }

        private void Start()
        {
            Assert.IsTrue(_weapon.WeaponData.AutoFire);
        }
        
        private void OnDisable()
        {
            _enemiesInRange = 0;
            _weapon.StopFiring();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & _enemyLayerMask) == 0)
            {
                return;
            }

            _enemiesInRange++;
            UpdateFiringState();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & _enemyLayerMask) == 0)
            {
                return;
            }

            _enemiesInRange = Mathf.Max(0, _enemiesInRange - 1);
            UpdateFiringState();
        }

        private void UpdateFiringState()
        {
            if (_enemiesInRange > 0)
            {
                _weapon.StartFiring();
            }
            else
            {
                _weapon.StopFiring();
            }
        }
    }
}