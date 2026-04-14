using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private PlayerStats _stats;

    [SerializeField]
    private Rigidbody2D _rigidBody;

    private Vector2 _movementInput = Vector2.zero;

    public void OnMove(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }

    private void Awake()
    {
        Assert.IsNotNull(_rigidBody);
    }

    private void FixedUpdate()
    {
        _rigidBody.MovePosition(
            _rigidBody.position + _movementInput * (_stats.MovementSpeed * Time.fixedDeltaTime)
        );
    }
}
