using Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace Enemy.Bosses
{
    public class OvenDamageZone : MonoBehaviour
    {
        [SerializeField] private int _damage = 3;
        private DamageDealer _damageDealer;

        private void Awake()
        {
            _damageDealer = GetComponent<DamageDealer>();
            Assert.IsNotNull(_damageDealer);

            _damageDealer.Activate(new DamageInfo(_damage, Vector2.zero));
        }
    }
}