using UnityEngine;
using UnityEngine.Assertions;

public class PlayerHeadController : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private SpriteRenderer _headRenderer;

    [Header("Head Sprites")] [SerializeField]
    private Sprite _up;

    [SerializeField] private Sprite _down;
    [SerializeField] private Sprite _side;

    private Camera _camera;

    private void Awake()
    {
        Assert.IsNotNull(_headRenderer);
        Assert.IsNotNull(_up);
        Assert.IsNotNull(_down);
        Assert.IsNotNull(_side);
        _camera = Camera.main;
    }

    public void UpdateHeadVisuals(Vector2 mouseInput)
    {
        Vector3 mouseScreenWithDepth = new Vector3(mouseInput.x, mouseInput.y, 10f);
        Vector3 mouseWorldPos = _camera.ScreenToWorldPoint(mouseScreenWithDepth);
        mouseWorldPos.z = 0;

        Vector2 direction = (mouseWorldPos - transform.position).normalized;

        if (direction.sqrMagnitude < 0.01f) return;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        UpdateHeadSpriteAndRotation(angle);
    }

    private void UpdateHeadSpriteAndRotation(float angle)
    {
        float visualAngle = angle;

        if (angle > -45 && angle <= 45)
        {
            _headRenderer.sprite = _side;
            _headRenderer.flipY = false;
        }
        else if (angle > 45 && angle <= 135)
        {
            _headRenderer.sprite = _up;
            _headRenderer.flipY = false;
            visualAngle = angle - 90f;
        }
        else if (angle > -135 && angle <= -45)
        {
            _headRenderer.sprite = _down;
            _headRenderer.flipY = false;
            visualAngle = angle + 90f;
        }
        else // Left
        {
            _headRenderer.sprite = _side;
            _headRenderer.flipY = true;
        }

        transform.rotation = Quaternion.Euler(0, 0, visualAngle);
    }
}