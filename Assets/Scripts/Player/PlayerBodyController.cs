using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerBodyController : MonoBehaviour
{
    [Header("Head Pivot Offsets")] [SerializeField]
    private Vector3 _upHeadOffset = new Vector3(0, 0.8f, 0);

    [SerializeField] private Vector3 _downHeadOffset = new Vector3(0, -0.5f, 0);
    [SerializeField] private Vector3 _rightHeadOffset = new Vector3(0.8f, 0.5f, 0);

    [Header("References")] [SerializeField]
    private PlayerHeadController _headController;

    [SerializeField] private Transform _headPivot;
    [SerializeField] private SpriteRenderer _headRenderer;
    [SerializeField] private PlayerItemSlotPosition _itemSlotPosition;

    [Header("Preference")] [SerializeField]
    private Animator _bodyAnimator;

    [SerializeField] private RuntimeAnimatorController _walkDown;
    [SerializeField] private RuntimeAnimatorController _walkRight;
    [SerializeField] private RuntimeAnimatorController _walkUp;
    [SerializeField] private RuntimeAnimatorController _walkLeft;

    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    private Vector2 _moveInput;
    private Vector2 _mouseInput;
    private Direction _currentDir = Direction.Right;
    private static readonly int Moving = Animator.StringToHash("isMoving");
    private Camera _camera;
    private const int HeadLayer = 14;

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _mouseInput = context.ReadValue<Vector2>();
    }

    private void Awake()
    {
        _camera = Camera.main;
        Assert.IsNotNull(_camera);
        Assert.IsNotNull(_headRenderer);
        Assert.IsNotNull(_headController);
        Assert.IsNotNull(_headPivot);
        Assert.IsNotNull(_bodyAnimator);
        Assert.IsNotNull(_walkDown);
        Assert.IsNotNull(_walkRight);
        Assert.IsNotNull(_walkUp);
        Assert.IsNotNull(_walkLeft);
    }

    private void Update()
    {
        var aimAngle = GetAngleToMouse();
        _bodyAnimator.SetBool(Moving, IsMoving());
        _headController.LookAtMouse(aimAngle);
        UpdateBodyVisuals(aimAngle);
    }

    private float GetAngleToMouse()
    {
        var mouseWorldPos = _camera.ScreenToWorldPoint(new Vector3(_mouseInput.x, _mouseInput.y, 10f));
        var direction = (Vector2)mouseWorldPos - (Vector2)transform.position;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    private bool IsMoving()
    {
        return _moveInput.sqrMagnitude > 0.01f;
    }

    private void UpdateBodyVisuals(float angle)
    {
        _currentDir = GetCardinalDirection(angle);
        var leftHeadOffset = new Vector3(-_rightHeadOffset.x, _rightHeadOffset.y, _rightHeadOffset.z);

        switch (_currentDir)
        {
            case Direction.Right:
                _bodyAnimator.runtimeAnimatorController = _walkRight;
                _itemSlotPosition.UpdateSlotLayout(Vector2.right, false);
                SetBodySpriteDirection(HeadLayer, _rightHeadOffset);
                break;
            case Direction.Up:
                _bodyAnimator.runtimeAnimatorController = _walkUp;
                _itemSlotPosition.UpdateSlotLayout(Vector2.up, false);
                SetBodySpriteDirection(HeadLayer, _upHeadOffset);
                break;
            case Direction.Down:
                _bodyAnimator.runtimeAnimatorController = _walkDown;
                _itemSlotPosition.UpdateSlotLayout(Vector2.down, false);
                SetBodySpriteDirection(HeadLayer, _downHeadOffset);
                break;
            case Direction.Left:
                _bodyAnimator.runtimeAnimatorController = _walkLeft;
                _itemSlotPosition.UpdateSlotLayout(Vector2.left, true);
                SetBodySpriteDirection(HeadLayer, leftHeadOffset);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SetBodySpriteDirection(int headOrder, Vector3 headPos)
    {
        UpdateHeadAnchor(headPos, headOrder);
    }

    private void UpdateHeadAnchor(Vector3 headPos, int headOrder)
    {
        _headRenderer.sortingOrder = headOrder;
        _headPivot.localPosition = headPos;
    }

    private static Direction GetCardinalDirection(float angle)
    {
        return angle switch
        {
            > -45f and <= 45f => Direction.Right,
            > 45f and <= 135f => Direction.Up,
            > -135f and <= -45f => Direction.Down,
            _ => Direction.Left
        };
    }
}