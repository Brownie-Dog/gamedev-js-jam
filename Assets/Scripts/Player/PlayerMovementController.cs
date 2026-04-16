using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField]
    private PlayerStatsSo _statsSo;

    [SerializeField]
    private Rigidbody2D _rigidBody;

    private Vector2 _movementInput = Vector2.zero;

    private void Awake()
    {
        Assert.IsNotNull(_statsSo);
        Assert.IsNotNull(_rigidBody);
    }

    private void FixedUpdate()
    {
        _rigidBody.MovePosition(
            _rigidBody.position + _movementInput * (_statsSo.MovementSpeed * Time.fixedDeltaTime)
        );
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }
}
