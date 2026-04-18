using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace ItemDrops
{
    public class DroppedItem : MonoBehaviour
    {
        [SerializeField]
        private Collider2D _collider;

        [SerializeField]
        private float _pickupDelay;

        private ItemDropManager _itemDropManager;

        private void Awake()
        {
            Assert.IsNotNull(_collider);

            _itemDropManager = GetComponentInParent<ItemDropManager>();
            Assert.IsNotNull(_itemDropManager);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(GlobalConstants.PLAYER_TAG))
            {
                return;
            }

            StartCoroutine(PickupAfterDelay());
        }

        private IEnumerator PickupAfterDelay()
        {
            yield return new WaitForSeconds(_pickupDelay);

            _itemDropManager.OnItemPickup(this.gameObject);
        }
    }
}
