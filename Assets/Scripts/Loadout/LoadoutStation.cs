using Player;
using UnityEngine;

namespace Loadout
{
    public class LoadoutStation : MonoBehaviour
    {
        [SerializeField] private CameraController _cameraController;

        [SerializeField] private LoadoutUI _loadoutUI;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(GlobalConstants.PLAYER_TAG))
            {
                return;
            }

            _cameraController.SetLoadoutOffset(true);
            _loadoutUI.Show();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(GlobalConstants.PLAYER_TAG))
            {
                return;
            }

            _cameraController.SetLoadoutOffset(false);
            _loadoutUI.Hide();
        }
    }
}