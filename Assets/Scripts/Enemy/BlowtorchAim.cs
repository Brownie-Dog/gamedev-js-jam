using UnityEngine;
using UnityEngine.Assertions;

public class BlowtorchAim : MonoBehaviour
{
    [Header("Visual References")] 
    [SerializeField] private Transform _weaponTransform;
    [SerializeField] private SpriteRenderer _botRenderer;
    [SerializeField] private SpriteRenderer _weaponRenderer;
    
    private void Awake()
    {
        Assert.IsNotNull(_weaponTransform);
        Assert.IsNotNull(_botRenderer);
        Assert.IsNotNull(_weaponRenderer);
    }

    private void Update()
    {
        float direction = _botRenderer.flipX ? -1f : 1f;

        Vector3 weaponPos = _weaponTransform.localPosition;
        weaponPos.x = Mathf.Abs(weaponPos.x) * direction;
        _weaponTransform.localPosition = weaponPos;

        Vector3 weaponScale = _weaponTransform.localScale;
        weaponScale.x = -direction;
        _weaponTransform.localScale = weaponScale;
    }
}