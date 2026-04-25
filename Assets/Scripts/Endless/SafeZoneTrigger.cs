using UnityEngine;

namespace EndlessMode
{
    public class SafeZoneTrigger : MonoBehaviour
    {
        [SerializeField] private EndlessModeManager _manager;
        [SerializeField] private LayerMask _playerLayer;

        private void OnTriggerExit2D(Collider2D other)
        {
            // Check if the thing leaving the safe zone is the player
            if (((1 << other.gameObject.layer) & _playerLayer) != 0)
            {
                _manager.StartNextWave();
            }
        }
    }
}