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
        Vector3 mouseWorldPos = _camera.ScreenToWorldPoint(new Vector3(mouseInput.x, mouseInput.y, 10f));
        Vector2 direction = (Vector2)mouseWorldPos - (Vector2)transform.position;

        if (IsCursorTooCloseToPivot(direction)) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        UpdateHeadRotation(angle);
    }

    private bool IsCursorTooCloseToPivot(Vector2 direction)
    {
        const float threshold = 0.01f;
        return direction.sqrMagnitude < threshold;
    }

    private void UpdateHeadRotation(float lookAngle)
    {
        bool isLookingLeft = Mathf.Abs(lookAngle) > 90f;
        _headRenderer.flipY = isLookingLeft;

        float finalRotation = isLookingLeft
            ? ConstrainNeckRotation(lookAngle, 180f)
            : ConstrainNeckRotation(lookAngle, 0f);

        transform.rotation = Quaternion.Euler(0, 0, finalRotation);
    }

    private float ConstrainNeckRotation(float currentAngle, float centerAxis)
    {
        float angleDifference = Mathf.DeltaAngle(centerAxis, currentAngle);
        float clampedDifference = Mathf.Clamp(angleDifference, -_maxRotationAngle, _maxRotationAngle);
        return centerAxis + clampedDifference;
    }
}