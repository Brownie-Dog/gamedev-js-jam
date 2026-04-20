using UnityEngine;
using UnityEngine.Assertions;

public class PlayerHeadController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _headRenderer;
    [SerializeField] private Sprite _up;
    [SerializeField] private Sprite _down;
    [SerializeField] private Sprite _side;
    [SerializeField, Range(0, 90)] private float _maxRotationAngle = 45f;

    private Camera _camera;

    private void Awake()
    {
        Assert.IsNotNull(_headRenderer);
        Assert.IsNotNull(_up);
        Assert.IsNotNull(_down);
        Assert.IsNotNull(_side);
        _camera = Camera.main;
        _headRenderer.sprite = _side;
    }

    public void LookAtMouse(Vector2 mouseInput)
    {
        var mouseWorldPos = _camera.ScreenToWorldPoint(new Vector3(mouseInput.x, mouseInput.y, 10f));
        var direction = (Vector2)mouseWorldPos - (Vector2)transform.position;

        if (IsCursorTooCloseToPivot(direction)) return;

        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        UpdateHeadRotation(angle);
    }

    private static bool IsCursorTooCloseToPivot(Vector2 direction)
    {
        const float threshold = 0.01f;
        return direction.sqrMagnitude < threshold;
    }

    private void UpdateHeadRotation(float lookAngle)
    {
        var isLookingLeft = Mathf.Abs(lookAngle) > 90f;
        _headRenderer.flipY = isLookingLeft;

        var finalRotation = isLookingLeft
            ? ConstrainNeckRotation(lookAngle, 180f)
            : ConstrainNeckRotation(lookAngle, 0f);

        transform.rotation = Quaternion.Euler(0, 0, finalRotation);
    }

    private float ConstrainNeckRotation(float currentAngle, float centerAxis)
    {
        var angleDifference = Mathf.DeltaAngle(centerAxis, currentAngle);
        var clampedDifference = Mathf.Clamp(angleDifference, -_maxRotationAngle, _maxRotationAngle);
        return centerAxis + clampedDifference;
    }
}