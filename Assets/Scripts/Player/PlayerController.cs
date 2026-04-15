using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

// TODO: Split this up further
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private PlayerStats _stats;

    public void OnLook(InputAction.CallbackContext context) { }

    public void OnPrimaryFire(InputAction.CallbackContext context) { }

    public void OnSecondaryFire(InputAction.CallbackContext context) { }

    public void OnLegButton(InputAction.CallbackContext context) { }
}
