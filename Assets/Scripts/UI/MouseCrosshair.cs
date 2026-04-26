using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class MouseCrosshair : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;

        [SerializeField] private float _rotationSpeed = 90f;

        private void Awake()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
        }

        private void OnEnable()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }

        private void OnDisable()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void Update()
        {
            HandlePosition();
            HandleRotation();
        }

        private void HandlePosition()
        {
            var mouse = Mouse.current;
            if (mouse == null)
            {
                return;
            }

            _rectTransform.position = mouse.position.ReadValue();
        }

        private void HandleRotation()
        {
            // Rotate the crosshair over time around the Z axis
            _rectTransform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);
        }
    }
}