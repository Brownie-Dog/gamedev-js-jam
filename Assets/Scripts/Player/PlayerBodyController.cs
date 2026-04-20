using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerBodyController : MonoBehaviour
{
    [Header("Body Config")] [SerializeField]
    private SpriteRenderer _bodyRenderer;

    [SerializeField] private Sprite _bodyUp, _bodyDown, _bodySide;

    [Header("Head Config")] [SerializeField]
    private PlayerHeadController _headController;

    [SerializeField] private Transform _headPivot;

    private Vector2 _moveInput;
    private Vector2 _mouseInput;

    private void Awake()
    {
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
        HandleBodyLogic();
        _headController.UpdateHeadVisuals(_mouseInput);
    }

    private void HandleBodyLogic()
    {
        if (_moveInput.sqrMagnitude < 0.01f) return;

        if (Mathf.Abs(_moveInput.y) > Mathf.Abs(_moveInput.x))
        {
            if (_moveInput.y > 0) SetBodyState(_bodyUp, false, 4);
            else SetBodyState(_bodyDown, false, 8);
        }
        else
        {
            bool flip = _moveInput.x < 0;
            SetBodyState(_bodySide, flip, 8);
        }
    }

    private void SetBodyState(Sprite s, bool flipX, int headOrder)
    {
        _bodyRenderer.sprite = s;
        _bodyRenderer.flipX = flipX;

        var headSR = _headPivot.GetComponentInChildren<SpriteRenderer>();
        headSR.sortingOrder = headOrder;

        var localPos = Vector3.zero;

        if (s == _bodyUp)
        {
            localPos = new Vector3(0, 1f, 0);
        }
        else if (s == _bodySide)
        {
            float xOffset = 1f;
            localPos = new Vector3(flipX ? -xOffset : xOffset, 0.5f, 0);
        }

        _headPivot.localPosition = localPos;
    }
}