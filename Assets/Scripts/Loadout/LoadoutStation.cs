using Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace Loadout
{
    public class LoadoutStation : MonoBehaviour
    {
        [SerializeField] private CameraController _cameraController;

        [SerializeField] private LoadoutUI _loadoutUI;
        
        [SerializeField] private PlayerBodyController _playerBody;

        private void Awake()
        {
            Assert.IsNotNull(_cameraController);
            Assert.IsNotNull(_loadoutUI);
            Assert.IsNotNull(_playerBody);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(GlobalConstants.PLAYER_TAG))
            {
                return;
            }
            
            DisableBodyController();
            _cameraController.SetLoadoutOffset(true);
            _loadoutUI.Show();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(GlobalConstants.PLAYER_TAG))
            {
                return;
            }
            
            EnableBodyController();
            _cameraController.SetLoadoutOffset(false);
            _loadoutUI.Hide();
        }

        private void DisableBodyController()
        {
            _playerBody.enabled = false;
        }

        private void EnableBodyController()
        {
            _playerBody.enabled = true;
        }
    }
}