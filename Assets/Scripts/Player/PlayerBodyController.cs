using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerBodyController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _bodyRenderer;
    [SerializeField] private Sprite _bodySide;
    [SerializeField] private PlayerHeadController _headController;
    [SerializeField] private Transform _headPivot;

    private Vector2 _moveInput;
    private Vector2 _mouseInput;
    private Vector3 _headPivotPosition;

    private void Awake()
    {
        Assert.IsNotNull(_bodyRenderer);
        Assert.IsNotNull(_bodySide);
        Assert.IsNotNull(_headController);
        Assert.IsNotNull(_headPivot);
        _headPivotPosition = _headPivot.localPosition;
        _bodyRenderer.sprite = _bodySide;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _mouseInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        UpdateBodyDirection();
        _headController.LookAtMouse(_mouseInput);
    }

    private void UpdateBodyDirection()
    {
        if (IsMovingHorizontally())
        {
            bool shouldFaceLeft = _moveInput.x < 0;
            FlipCharacter(shouldFaceLeft);
        }
    }

    private bool IsMovingHorizontally()
    {
        return Mathf.Abs(_moveInput.x) > 0.01f;
    }

    private void FlipCharacter(bool left)
    {
        _bodyRenderer.flipX = left;
        float mirroredX = left ? -_headPivotPosition.x : _headPivotPosition.x;
        _headPivot.localPosition = new Vector3(mirroredX, _headPivotPosition.y, _headPivotPosition.z);
    }
}