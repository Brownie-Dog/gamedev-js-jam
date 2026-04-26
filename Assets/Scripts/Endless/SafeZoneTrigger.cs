using UnityEngine;
using UnityEngine.Assertions;

namespace EndlessMode
{
    public enum TriggerAxis { X_Axis, Y_Axis }

    public class SafeZoneTrigger : MonoBehaviour
    {
        [SerializeField] private EndlessModeManager _manager;
        [SerializeField] private LayerMask _playerLayer;
        
        [Header("Detection Settings")]
        [SerializeField] private TriggerAxis _axisToTrack = TriggerAxis.X_Axis;
        [Tooltip("If true, exiting towards positive (Right/Up) starts wave. If false, negative (Left/Down) starts it.")]
        [SerializeField] private bool _positiveIsArena = true;

        private void Awake()
        {
            Assert.IsNotNull(_manager);
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & _playerLayer) != 0)
            {
                float playerPos = (_axisToTrack == TriggerAxis.X_Axis) ? other.transform.position.x : other.transform.position.y;
                float triggerPos = (_axisToTrack == TriggerAxis.X_Axis) ? transform.position.x : transform.position.y;

                // Check if the player is now on the "Arena Side" of the line
                bool isOnArenaSide = _positiveIsArena ? (playerPos > triggerPos) : (playerPos < triggerPos);

                if (isOnArenaSide)
                {
                    _manager.StartNextWave();
                }
            }
        }
    }
}