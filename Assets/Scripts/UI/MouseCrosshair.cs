using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class MouseCrosshair : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;

    void Awake()
    {
        Assert.IsNotNull(_rectTransform);
        Assert.IsNotNull(Mouse.current);
        Cursor.visible = false;
    }

    void Update()
    {
        _rectTransform.position = Mouse.current.position.ReadValue();
    }
}