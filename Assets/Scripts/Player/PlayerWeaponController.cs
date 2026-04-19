using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Weapons;

namespace Player
{
    public class PlayerWeaponController : MonoBehaviour
    {
        public EventHandler PrimaryFireTriggered;
        public EventHandler SecondaryFireTriggered;
        public EventHandler<AimDirectionArgs> AimDirectionUpdated;

        [SerializeField] private Camera _mainCamera;

        private void Update()
        {
            var mouseScreenPos = Mouse.current.position.ReadValue();
            var mouseWorldPos = _mainCamera.ScreenToWorldPoint(mouseScreenPos);
            var direction = ((Vector2)mouseWorldPos - (Vector2)transform.position).normalized;
            AimDirectionUpdated?.Invoke(this, new AimDirectionArgs(direction));
        }

        public void InputEvent_OnPrimaryFire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                PrimaryFireTriggered?.Invoke(this, EventArgs.Empty);
            }
        }

        public void InputEvent_OnSecondaryFire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                SecondaryFireTriggered?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
