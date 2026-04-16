using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
public class Health : MonoBehaviour
{
    [SerializeField]
    private PlayerStats _stats;

    private int _currentHealth = 0;

    public void OnMove(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }

    private void Awake()
    {
        // Assert.IsNotNull(_rigidBody);
        _currentHealth = _stats.MaxHealth;
    }

    private void FixedUpdate()
    {
        _rigidBody.MovePosition(
            _rigidBody.position + _movementInput * (_stats.MovementSpeed * Time.fixedDeltaTime)
        );
    }
}
