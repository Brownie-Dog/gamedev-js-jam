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
        [SerializeField] private float _rotationOffset;

        private Weapon _weapon;
        private PlayerWeaponController _weaponController;
        private AimMode _aimMode;
        private Vector3 _manualAimTargetPosition;

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

            _weapon = GetComponentInParent<Weapon>();
            Assert.IsNotNull(_weapon);

            _weaponController = GetComponentInParent<PlayerWeaponController>();
            Assert.IsNotNull(_weaponController);
        }

        private void Start()
        {
            _aimMode = _weapon.WeaponData.DefaultAimMode;
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
            _manualAimTargetPosition = args.TargetPosition;
        }

        private void LateUpdate()
        {
            switch (_aimMode)
            {
                case AimMode.Manual:
                    Vector2 individualDirection = (_manualAimTargetPosition - transform.position).normalized;
                    RotateTowards(individualDirection);
                    break;
                case AimMode.AutoAim:
                    var enemyDir = FindNearestEnemyDirection();
                    if (enemyDir.HasValue)
                    {
                        RotateTowards(enemyDir.Value);
                    }
                    else
                    {
                        Vector2 fallbackDir = (_manualAimTargetPosition - transform.position).normalized;
                        RotateTowards(fallbackDir);
                    }
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