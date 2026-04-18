using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerWeaponController : MonoBehaviour
    {
        public EventHandler PrimaryFireTriggered;
        public EventHandler SecondaryFireTriggered;

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
