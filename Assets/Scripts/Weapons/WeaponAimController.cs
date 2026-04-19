using System;
using Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace Weapons
{
    public class WeaponAimController : MonoBehaviour
    {
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private CircleCollider2D _autoAimDetectionRadius;
        [SerializeField] private Transform _manualAimRotateRoot;
        [SerializeField] private ItemDrops.WeaponItemData _weaponData;
        [SerializeField] private float _rotationOffset;

        private PlayerWeaponController _weaponController;
        private AimMode _aimMode;
        private Vector2 _manualAimDirection;

        public AimMode Mode
        {
            get => _aimMode;
            set => _aimMode = value;
        }

        private void Awake()
        {
            Assert.IsNotNull(_autoAimDetectionRadius);
            _autoAimDetectionRadius.isTrigger = true;

            Assert.IsNotNull(_manualAimRotateRoot);

            _weaponController = GetComponentInParent<PlayerWeaponController>();
            Assert.IsNotNull(_weaponController);

            Assert.IsNotNull(_weaponData);
            _aimMode = _weaponData.DefaultAimMode;
        }

        private void OnEnable()
        {
            _weaponController.AimDirectionUpdated += OnAimDirectionUpdated;
        }

        private void OnDisable()
        {
            _weaponController.AimDirectionUpdated -= OnAimDirectionUpdated;
        }

        private void OnAimDirectionUpdated(object sender, AimDirectionArgs args)
        {
            _manualAimDirection = args.Direction;
        }

        private void LateUpdate()
        {
            switch (_aimMode)
            {
                case AimMode.Manual:
                    RotateTowards(_manualAimDirection);
                    break;
                case AimMode.AutoAim:
                    var enemyDir = FindNearestEnemyDirection();
                    RotateTowards(enemyDir.HasValue ? enemyDir.Value : _manualAimDirection);
                    break;
                case AimMode.Directional:
                default:
                    break;
            }
        }

        private void RotateTowards(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.001f)
            {
                return;
            }

            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _manualAimRotateRoot.rotation = Quaternion.Euler(0f, 0f, angle + _rotationOffset);
        }

        private Vector2? FindNearestEnemyDirection()
        {
            var radius = _autoAimDetectionRadius.radius;
            var hits = Physics2D.OverlapCircleAll(transform.position, radius, _enemyLayer);
            var nearestDistance = float.MaxValue;
            Vector2? nearestDirection = null;

            foreach (var hit in hits)
            {
                var damageable = hit.GetComponent<IDamageable>();
                if (damageable == null)
                {
                    continue;
                }

                var direction = (Vector2)(hit.transform.position - transform.position);
                var distance = direction.sqrMagnitude;

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestDirection = direction.normalized;
                }
            }

            return nearestDirection;
        }

        private void OnDrawGizmosSelected()
        {
            if (_manualAimRotateRoot == null)
            {
                return;
            }

            var offsetRad = _rotationOffset * Mathf.Deg2Rad;
            var dir = new Vector2(Mathf.Cos(offsetRad), Mathf.Sin(offsetRad));
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(_manualAimRotateRoot.position, dir * 2f);
        }
    }
}