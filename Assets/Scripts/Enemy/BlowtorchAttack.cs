using UnityEngine;
using UnityEngine.Assertions;

public class BlowtorchAttack : RangedEnemyAttack
{
    [SerializeField] private Transform _weaponTransform;
    [SerializeField] private SpriteRenderer _botRenderer;
    [SerializeField] private SpriteRenderer _weaponRenderer;

    protected override void Awake()
    {
        base.Awake();
        Assert.IsNotNull(_weaponTransform);
        Assert.IsNotNull(_botRenderer);
        Assert.IsNotNull(_weaponRenderer);
    }

    protected override void Update()
    {
        if (_player != null) 
        {
            AimWeapon();
        }
        base.Update();
    }

    private void AimWeapon()
    {
        float direction = _botRenderer.flipX ? 1f : -1f;

        Vector3 weaponPos = _weaponTransform.localPosition;
        weaponPos.x = Mathf.Abs(weaponPos.x) * direction;
        _weaponTransform.localPosition = weaponPos;
        _weaponRenderer.flipX = _botRenderer.flipX;

        Vector3 targetDir = _player.position - _weaponTransform.position;
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

        if (direction == -1f) angle += 180f;

        _weaponTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}