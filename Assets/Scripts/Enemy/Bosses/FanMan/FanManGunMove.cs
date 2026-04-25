using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Weapons;
using Random = UnityEngine.Random;

namespace Enemy.Bosses
{
    public class FanManGunMove : MonoBehaviour, IFanManMove
    {
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private int _bulletPoolSize = 20;
        [SerializeField] private EnemyStats _stats;
        [SerializeField] private FanManRailgunBeamSegment _railgunBeamPrefab;
        [SerializeField] private int _railgunBeamPoolSize = 5;

        [Header("Left Hand Guns")]
        [SerializeField] private GameObject _leftNormalGun1;
        [SerializeField] private Transform _leftNormalGun1FirePoint;
        [SerializeField] private GameObject _leftNormalGun2;
        [SerializeField] private Transform _leftNormalGun2FirePoint;
        [SerializeField] private GameObject _leftRailgun;
        [SerializeField] private Transform _leftRailgunFirePoint;

        [Header("Right Hand Guns")]
        [SerializeField] private GameObject _rightNormalGun1;
        [SerializeField] private Transform _rightNormalGun1FirePoint;
        [SerializeField] private GameObject _rightNormalGun2;
        [SerializeField] private Transform _rightNormalGun2FirePoint;
        [SerializeField] private GameObject _rightRailgun;
        [SerializeField] private Transform _rightRailgunFirePoint;

        private MonoBehaviourPool<Bullet> _bulletPool;
        private MonoBehaviourPool<FanManRailgunBeamSegment> _beamPool;

        private int _shotMin = 2;
        private int _shotMax = 3;
        private float _shotInterval = 0.2f;
        private float _aimMin = 2f;
        private float _aimMax = 5f;
        private float _railgunTelegraph = 2f;
        private float _railgunLinger = 1.5f;
        private float _railgunMaxRange = 30f;
        private float _normalGunWeight = 0.5f;
        private float _railgunWeight = 0f;

        private Coroutine _routine;
        private GameObject _activeTelegraph;
        private GameObject _activeGun;
        private bool _isComplete;
        private bool _isActive;

        public bool IsComplete => _isComplete;
        public bool IsActive => _isActive;
        public event Action OnMoveComplete;

        private void Awake()
        {
            Assert.IsNotNull(_bulletPrefab);
            Assert.IsNotNull(_stats);
            Assert.IsNotNull(_railgunBeamPrefab);

            _bulletPool = new MonoBehaviourPool<Bullet>(_bulletPrefab, _bulletPoolSize, _bulletPoolSize, transform,
                b => b.SetPool(_bulletPool.Pool)
            );
            _beamPool = new MonoBehaviourPool<FanManRailgunBeamSegment>(_railgunBeamPrefab, _railgunBeamPoolSize, _railgunBeamPoolSize, transform,
                b => b.SetPool(_beamPool.Pool)
            );

            DisableAllGuns();
        }

        private void OnDisable()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }
            if (_activeTelegraph != null)
            {
                Destroy(_activeTelegraph);
                _activeTelegraph = null;
            }
            DisableAllGuns();
        }

        private void DisableAllGuns()
        {
            if (_leftNormalGun1 != null)
            {
                _leftNormalGun1.SetActive(false);
            }

            if (_leftNormalGun2 != null)
            {
                _leftNormalGun2.SetActive(false);
            }

            if (_leftRailgun != null)
            {
                _leftRailgun.SetActive(false);
            }

            if (_rightNormalGun1 != null)
            {
                _rightNormalGun1.SetActive(false);
            }

            if (_rightNormalGun2 != null)
            {
                _rightNormalGun2.SetActive(false);
            }

            if (_rightRailgun != null)
            {
                _rightRailgun.SetActive(false);
            }
        }

        public void SetShotCountRange(int min, int max)
        {
            _shotMin = min;
            _shotMax = max;
        }

        public void SetShotInterval(float interval)
        {
            _shotInterval = interval;
        }

        public void SetAimDurationRange(float min, float max)
        {
            _aimMin = min;
            _aimMax = max;
        }

        public void SetRailgunStats(float telegraph, float linger, float maxRange)
        {
            _railgunTelegraph = telegraph;
            _railgunLinger = linger;
            _railgunMaxRange = maxRange;
        }

        public void SetGunWeights(float normalGunWeight, float railgunWeight)
        {
            _normalGunWeight = normalGunWeight;
            _railgunWeight = railgunWeight;
        }

        public void Execute(Transform boss, Transform player)
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
            }
            _isComplete = false;
            _isActive = true;
            _routine = StartCoroutine(ExecuteCore(boss, player));
        }

        private IEnumerator ExecuteCore(Transform boss, Transform player)
        {
            bool useLeftHand = Random.value < 0.5f;
            float totalGunWeight = _normalGunWeight + _railgunWeight;
            float roll = Random.value * totalGunWeight;
            bool useRailgun = roll >= _normalGunWeight && _railgunWeight > 0f;

            if (useRailgun)
            {
                yield return ExecuteRailgun(player, useLeftHand);
            }
            else
            {
                yield return ExecuteNormalGun(player, useLeftHand);
            }

            DisableAllGuns();
            _activeGun = null;
            _isActive = false;
            _isComplete = true;
            OnMoveComplete?.Invoke();
            _routine = null;
        }

        private IEnumerator ExecuteNormalGun(Transform player, bool useLeftHand)
        {
            bool useGun1 = Random.value < 0.5f;

            _activeGun = useLeftHand
                ? (useGun1 ? _leftNormalGun1 : _leftNormalGun2)
                : (useGun1 ? _rightNormalGun1 : _rightNormalGun2);

            Transform firePoint = useLeftHand
                ? (useGun1 ? _leftNormalGun1FirePoint : _leftNormalGun2FirePoint)
                : (useGun1 ? _rightNormalGun1FirePoint : _rightNormalGun2FirePoint);

            if (_activeGun != null)
            {
                _activeGun.SetActive(true);
            }

            // Aim toward player
            float aimDuration = Random.Range(_aimMin, _aimMax);
            float aimTimer = 0f;
            while (aimTimer < aimDuration)
            {
                aimTimer += Time.deltaTime;
                if (firePoint != null)
                {
                    Vector2 direction = ((Vector2)(player.position - firePoint.position)).normalized;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                    firePoint.rotation = Quaternion.Euler(0, 0, angle);
                }
                yield return null;
            }

            int shots = Random.Range(_shotMin, _shotMax + 1);
            for (int i = 0; i < shots; i++)
            {
                if (firePoint != null)
                {
                    Vector2 dir = ((Vector2)(player.position - firePoint.position)).normalized;
                    var damageInfo = new DamageInfo(_stats.Damage, dir * _stats.KnockbackForce);
                    Bullet bullet = _bulletPool.Get();
                    bullet.Activate(firePoint.position, dir, damageInfo, 1);
                }
                yield return new WaitForSeconds(_shotInterval);
            }
        }

        private IEnumerator ExecuteRailgun(Transform player, bool useLeftHand)
        {
            _activeGun = useLeftHand ? _leftRailgun : _rightRailgun;
            if (_activeGun != null)
            {
                _activeGun.SetActive(true);
            }

            Transform firePoint = useLeftHand ? _leftRailgunFirePoint : _rightRailgunFirePoint;
            Vector2 start = firePoint != null ? (Vector2)firePoint.position : (Vector2)transform.position;
            Vector2 dir = ((Vector2)(player.position - (Vector3)start)).normalized;

            // Telegraph line
            GameObject telegraphObj = new GameObject("RailgunTelegraph");
            _activeTelegraph = telegraphObj;
            LineRenderer telegraph = telegraphObj.AddComponent<LineRenderer>();
            telegraph.useWorldSpace = true;
            telegraph.positionCount = 2;
            telegraph.startWidth = 0.05f;
            telegraph.endWidth = 0.05f;
            telegraph.SetPosition(0, start);
            telegraph.SetPosition(1, start + dir * _railgunMaxRange);
            telegraph.startColor = Color.red;
            telegraph.endColor = new Color(1f, 0f, 0f, 0.3f);

            yield return new WaitForSeconds(_railgunTelegraph);

            if (_activeTelegraph != null)
            {
                Destroy(_activeTelegraph);
                _activeTelegraph = null;
            }

            // Fire
            FireRailgun(start, dir);
        }

        private void FireRailgun(Vector2 start, Vector2 direction)
        {
            float remainingRange = _railgunMaxRange;
            Vector2 currentStart = start;
            Vector2 currentDir = direction;

            RaycastHit2D hit = Physics2D.Raycast(currentStart, currentDir, remainingRange, _stats.WallLayer);
            Vector2 end;
            bool bounced = false;
            Vector2 bounceDir = Vector2.zero;
            Vector2 bounceEnd = Vector2.zero;

            if (hit.collider != null)
            {
                end = hit.point;
                remainingRange -= hit.distance;

                bounceDir = Vector2.Reflect(currentDir, hit.normal);
                RaycastHit2D bounceHit = Physics2D.Raycast(end, bounceDir, remainingRange, _stats.WallLayer);
                bounceEnd = bounceHit.collider != null ? bounceHit.point : end + bounceDir * remainingRange;
                bounced = true;
            }
            else
            {
                end = currentStart + currentDir * remainingRange;
            }

            // Spawn segment 1
            FanManRailgunBeamSegment seg1 = _beamPool.Get();
            seg1.transform.SetParent(null);
            seg1.transform.localScale = Vector3.one;
            seg1.Setup(currentStart, end, _railgunLinger, _stats.Damage, _stats.KnockbackForce);

            if (bounced)
            {
                FanManRailgunBeamSegment seg2 = _beamPool.Get();
                seg2.transform.SetParent(null);
                seg2.transform.localScale = Vector3.one;
                seg2.Setup(end, bounceEnd, _railgunLinger, _stats.Damage, _stats.KnockbackForce);
            }
        }
    }
}
