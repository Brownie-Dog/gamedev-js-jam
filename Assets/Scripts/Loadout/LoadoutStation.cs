using Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace Loadout
{
    public class LoadoutStation : MonoBehaviour
    {
        [SerializeField] private CameraController _cameraController;

        [SerializeField] private LoadoutUI _loadoutUI;

        private void Awake()
        {
            Assert.IsNotNull(_cameraController);
            Assert.IsNotNull(_loadoutUI);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(GlobalConstants.PLAYER_TAG))
            {
                return;
            }

            DisableBodyController(other.GetComponent<PlayerBodyController>());
            _cameraController.SetLoadoutOffset(true);
            _loadoutUI.Show();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(GlobalConstants.PLAYER_TAG))
            {
                return;
            }

            EnableBodyController(other.GetComponent<PlayerBodyController>());
            _cameraController.SetLoadoutOffset(false);
            _loadoutUI.Hide();
        }

        private static void DisableBodyController(PlayerBodyController bodyController)
        {
            bodyController.enabled = false;
        }

        private static void EnableBodyController(PlayerBodyController bodyController)
        {
            bodyController.enabled = true;
        }
    }
}