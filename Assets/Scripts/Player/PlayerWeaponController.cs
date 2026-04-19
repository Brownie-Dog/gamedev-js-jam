using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Weapons;

namespace Player
{
    public class PlayerWeaponController : MonoBehaviour
    {
        public EventHandler PrimaryFireStarted;
        public EventHandler PrimaryFireCanceled;
        public EventHandler SecondaryFireStarted;
        public EventHandler SecondaryFireCanceled;
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
            if (context.started)
            {
                PrimaryFireStarted?.Invoke(this, EventArgs.Empty);
            }
            else if (context.canceled)
            {
                PrimaryFireCanceled?.Invoke(this, EventArgs.Empty);
            }
        }

        public void InputEvent_OnSecondaryFire(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                SecondaryFireStarted?.Invoke(this, EventArgs.Empty);
            }
            else if (context.canceled)
            {
                SecondaryFireCanceled?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}