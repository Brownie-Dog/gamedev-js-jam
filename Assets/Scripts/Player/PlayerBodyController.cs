using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerBodyController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _bodyRenderer;
    [SerializeField] private Sprite _bodyUp, _bodyDown, _bodySide;
    [SerializeField] private PlayerHeadController _headController;
    [SerializeField] private Transform _headPivot;

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
    private SpriteRenderer _headRenderer;

    private void Awake()
    {
        _headRenderer = _headPivot.GetComponentInChildren<SpriteRenderer>();
        Assert.IsNotNull(_headRenderer);
        Assert.IsNotNull(_bodyRenderer);
        Assert.IsNotNull(_bodyUp);
        Assert.IsNotNull(_bodyDown);
        Assert.IsNotNull(_bodySide);
        Assert.IsNotNull(_headController);
        Assert.IsNotNull(_headPivot);
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
        var aimAngle = GetAngleToMouse();
        _headController.LookAtMouse(aimAngle);
        UpdateBodyVisuals(aimAngle);
    }

    private float GetAngleToMouse()
    {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(_mouseInput.x, _mouseInput.y, 10f));
        var direction = (Vector2)mouseWorldPos - (Vector2)transform.position;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    private void UpdateBodyVisuals(float angle)
    {
        _currentDir = GetCardinalDirection(angle);
        switch (_currentDir)
        {
            case Direction.Right:
                SetBodySpriteDirection(_bodySide, false, 8, new Vector3(1f, 0.5f, 0));
                break;
            case Direction.Up:
                SetBodySpriteDirection(_bodyUp, false, 4, new Vector3(0, 1f, 0));
                break;
            case Direction.Down:
                SetBodySpriteDirection(_bodyDown, false, 8, new Vector3(0, 0.3f, 0));
                break;
            case Direction.Left:
                SetBodySpriteDirection(_bodySide, true, 8, new Vector3(-1f, 0.5f, 0));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SetBodySpriteDirection(Sprite sprite, bool flipX, int headOrder, Vector3 headPos)
    {
        _bodyRenderer.sprite = sprite;
        _bodyRenderer.flipX = flipX;
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