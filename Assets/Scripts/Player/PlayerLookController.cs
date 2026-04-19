using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerLookController : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [SerializeField] private Transform _visualPivot;

    [Header("Sprites")] [SerializeField] private Sprite _up;
    [SerializeField] private Sprite _down;
    [SerializeField] private Sprite _side;

    private Vector2 _mouseInput;
    private Camera _camera;

    private void Awake()
    {
        Assert.IsNotNull(_spriteRenderer);
        Assert.IsNotNull(_visualPivot);
        Assert.IsNotNull(_up);
        Assert.IsNotNull(_down);
        Assert.IsNotNull(_side);
        _camera = Camera.main;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _mouseInput = context.ReadValue<Vector2>();
    }

    void Update()
    {
        HandleRotationAndSprite();
    }

    private void HandleRotationAndSprite()
    {
        Vector3 mouseScreenWithDepth = new Vector3(_mouseInput.x, _mouseInput.y, 10f);
        Vector3 mouseWorldPos = _camera.ScreenToWorldPoint(mouseScreenWithDepth);
        mouseWorldPos.z = 0;

        Vector2 direction = (mouseWorldPos - transform.position).normalized;

        if (direction.sqrMagnitude < 0.01f) return;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        _visualPivot.rotation = Quaternion.Euler(0, 0, angle);

        UpdateSpriteSelection(angle);
    }

    private void UpdateSpriteSelection(float angle)
    {
        float visualAngle = angle;

        if (angle > -45 && angle <= 45)
        {
            _spriteRenderer.sprite = _side;
            _spriteRenderer.flipY = false;
        }
        else if (angle > 45 && angle <= 135)
        {
            _spriteRenderer.sprite = _up;
            _spriteRenderer.flipY = false;
            visualAngle = angle - 90f;
        }
        else if (angle > -135 && angle <= -45)
        {
            _spriteRenderer.sprite = _down;
            _spriteRenderer.flipY = false;
            visualAngle = angle + 90f;
        }
        else
        {
            _spriteRenderer.sprite = _side;
            _spriteRenderer.flipY = true;
        }

        _visualPivot.rotation = Quaternion.Euler(0, 0, visualAngle);
    }
}