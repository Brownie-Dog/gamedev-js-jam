using UnityEngine;
using UnityEngine.Assertions;

public class PlayerHeadController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _headRenderer;
    [SerializeField] private Sprite _up;
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
    
    public void LookAtMouse(float angle)
    {
        UpdateHeadSprite(angle);
    }

    private void UpdateHeadSprite(float angle)
    {
        var visualAngle = angle;

        switch (angle)
        {
            case > -45 and <= 45:
                _headRenderer.sprite = _side;
                _headRenderer.flipY = false;
                break;
            case > 45 and <= 135:
                _headRenderer.sprite = _up;
                _headRenderer.flipY = false;
                visualAngle = angle - 90f;
                break;
            case > -135 and <= -45:
                _headRenderer.sprite = _down;
                _headRenderer.flipY = false;
                visualAngle = angle + 90f;
                break;
            default:
                _headRenderer.sprite = _side;
                _headRenderer.flipY = true;
                break;
        }

        transform.rotation = Quaternion.Euler(0, 0, visualAngle);
    }
}