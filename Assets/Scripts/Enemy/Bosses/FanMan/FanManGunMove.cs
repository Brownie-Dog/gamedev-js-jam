using System;
using System.Collections;
using System.Collections.Generic;
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
        private int _railgunMultiShotMin = 1;
        private int _railgunMultiShotMax = 1;
        private float _railgunTelegraphInterval = 0.5f;
        private float _railgunAimVarianceDeg = 0f;
        private float _railgunAimVarianceThreshold = 1f;
        private float _normalGunWeight = 0.5f;
        private float _railgunWeight = 0f;
        private bool _useBothHands;

        private Coroutine _routine;
        private readonly List<GameObject> _activeTelegraphs = new();
        private readonly List<GameObject> _activeGuns = new();
        private int _activeBeamSegments;
        private bool _isComplete;
        private bool _isActive;

        public bool IsComplete => _isComplete && _activeBeamSegments == 0;
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
            ClearTelegraphs();
            DisableAllGuns();
        }

        private void ClearTelegraphs()
        {
            foreach (var t in _activeTelegraphs)
            {
                if (t != null)
                {
                    Destroy(t);
                }
            }
            _activeTelegraphs.Clear();
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

            _activeGuns.Clear();
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

        public void SetMultiShotConfig(int min, int max, float interval, float varianceDeg, float varianceThreshold)
        {
            _railgunMultiShotMin = min;
            _railgunMultiShotMax = max;
            _railgunTelegraphInterval = interval;
            _railgunAimVarianceDeg = varianceDeg;
            _railgunAimVarianceThreshold = varianceThreshold;
        }

        public void SetUseBothHands(bool useBoth)
        {
            _useBothHands = useBoth;
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
            float totalGunWeight = _normalGunWeight + _railgunWeight;
            float roll = Random.value * totalGunWeight;
            bool useRailgun = roll >= _normalGunWeight && _railgunWeight > 0f;

            if (useRailgun)
            {
                // Pause movement for the entire railgun sequence (aim + telegraph + fire + linger)
                var movement = boss.GetComponent<EnemyMovement>();
                if (movement != null)
                {
                    movement.PauseMovement();
                }

                yield return ExecuteRailgun(player, _useBothHands);

                while (_activeBeamSegments > 0)
                {
                    yield return null;
                }

                if (movement != null)
                {
                    movement.ResumeMovement();
                }
            }
            else
            {
                yield return ExecuteNormalGun(player, _useBothHands);
            }

            DisableAllGuns();
            _isActive = false;
            _isComplete = true;
            OnMoveComplete?.Invoke();
            _routine = null;
        }

        private void ActivateGun(GameObject gun)
        {
            if (gun != null)
            {
                gun.SetActive(true);
                _activeGuns.Add(gun);
            }
        }

        private IEnumerator ExecuteNormalGun(Transform player, bool dualHand)
        {
            // Pick guns for each hand
            bool leftGun1 = Random.value < 0.5f;
            bool rightGun1 = Random.value < 0.5f;

            GameObject leftGun = leftGun1 ? _leftNormalGun1 : _leftNormalGun2;
            GameObject rightGun = rightGun1 ? _rightNormalGun1 : _rightNormalGun2;
            Transform leftFire = leftGun1 ? _leftNormalGun1FirePoint : _leftNormalGun2FirePoint;
            Transform rightFire = rightGun1 ? _rightNormalGun1FirePoint : _rightNormalGun2FirePoint;

            if (dualHand)
            {
                ActivateGun(leftGun);
                ActivateGun(rightGun);
            }
            else
            {
                bool useLeft = Random.value < 0.5f;
                ActivateGun(useLeft ? leftGun : rightGun);
            }

            // Aim phase
            float aimDuration = Random.Range(_aimMin, _aimMax);
            float aimTimer = 0f;
            while (aimTimer < aimDuration)
            {
                aimTimer += Time.deltaTime;
                AimFirePoint(leftFire, player);
                if (dualHand)
                {
                    AimFirePoint(rightFire, player);
                }

                yield return null;
            }

            int shots = Random.Range(_shotMin, _shotMax + 1);

            if (dualHand)
            {
                bool simultaneous = Random.value < 0.5f;

                if (simultaneous)
                {
                    // Both hands fire together for each shot
                    for (int i = 0; i < shots; i++)
                    {
                        FireBullet(leftFire, player);
                        FireBullet(rightFire, player);
                        yield return new WaitForSeconds(_shotInterval);
                    }
                }
                else
                {
                    // Alternating fire
                    bool leftFirst = Random.value < 0.5f;
                    for (int i = 0; i < shots; i++)
                    {
                        FireBullet(leftFirst ? leftFire : rightFire, player);
                        yield return new WaitForSeconds(_shotInterval);
                        FireBullet(leftFirst ? rightFire : leftFire, player);
                        yield return new WaitForSeconds(_shotInterval);
                    }
                }
            }
            else
            {
                // Single hand
                Transform activeFire = _activeGuns.Count > 0 && _activeGuns[0] == leftGun ? leftFire : rightFire;
                if (activeFire == null)
                {
                    activeFire = leftFire != null ? leftFire : rightFire;
                }
                for (int i = 0; i < shots; i++)
                {
                    FireBullet(activeFire, player);
                    yield return new WaitForSeconds(_shotInterval);
                }
            }
        }

        private IEnumerator ExecuteRailgun(Transform player, bool dualHand)
        {
            Transform activeFire;
            Transform secondaryFire = null;

            if (dualHand)
            {
                ActivateGun(_leftRailgun);
                ActivateGun(_rightRailgun);
                activeFire = _leftRailgunFirePoint;
                secondaryFire = _rightRailgunFirePoint;
            }
            else
            {
                bool useLeft = Random.value < 0.5f;
                ActivateGun(useLeft ? _leftRailgun : _rightRailgun);
                activeFire = useLeft ? _leftRailgunFirePoint : _rightRailgunFirePoint;
            }

            int shotCount = Mathf.Max(1, Random.Range(_railgunMultiShotMin, _railgunMultiShotMax + 1));

            // Initial aim phase
            float aimDuration = Random.Range(_aimMin, _aimMax);
            yield return AimPhaseCoroutine(activeFire, secondaryFire, player, aimDuration);

            // Collect locked shots per hand
            var activeShots = new List<(Vector2 start, Vector2 dir)>();
            var secondaryShots = new List<(Vector2 start, Vector2 dir)>();

            Vector2 lastActiveCheck = player.position;
            Vector2 lastSecondaryCheck = player.position;

            for (int shot = 0; shot < shotCount; shot++)
            {
                // Lock aim for this shot
                if (activeFire != null)
                {
                    Vector2 start = activeFire.position;
                    Vector2 dir = CalculateAimDirection(activeFire, player, lastActiveCheck, shot == 0);
                    lastActiveCheck = player.position;
                    activeShots.Add((start, dir));
                    CreateTelegraph(start, dir);
                }
                if (secondaryFire != null)
                {
                    Vector2 start = secondaryFire.position;
                    Vector2 dir = CalculateAimDirection(secondaryFire, player, lastSecondaryCheck, shot == 0);
                    lastSecondaryCheck = player.position;
                    secondaryShots.Add((start, dir));
                    CreateTelegraph(start, dir);
                }

                yield return new WaitForSeconds(_railgunTelegraph);

                // Re-aim and wait interval before next telegraph (if not last shot)
                if (shot < shotCount - 1)
                {
                    yield return AimPhaseCoroutine(activeFire, secondaryFire, player, _railgunTelegraphInterval);
                }
            }

            // Clear all telegraphs before firing
            ClearTelegraphs();

            // Fire all shots in sequence with tiny delay
            for (int i = 0; i < activeShots.Count; i++)
            {
                FireRailgun(activeShots[i].start, activeShots[i].dir);
                if (dualHand && i < secondaryShots.Count)
                {
                    FireRailgun(secondaryShots[i].start, secondaryShots[i].dir);
                }

                yield return new WaitForSeconds(0.2f);
            }
        }

        private IEnumerator AimPhaseCoroutine(Transform fireA, Transform fireB, Transform player, float duration)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                if (fireA != null)
                {
                    AimFirePoint(fireA, player);
                }

                if (fireB != null)
                {
                    AimFirePoint(fireB, player);
                }

                yield return null;
            }
        }

        private Vector2 CalculateAimDirection(Transform firePoint, Transform player, Vector2 lastCheckedPos, bool isFirstShot)
        {
            Vector2 rawDir = ((Vector2)(player.position - firePoint.position)).normalized;

            // First shot is always accurate; variance only applies to follow-up shots
            if (isFirstShot || _railgunAimVarianceDeg <= 0f)
            {
                return rawDir;
            }

            // Add random angular offset
            float offset = Random.Range(-_railgunAimVarianceDeg, _railgunAimVarianceDeg);
            return Quaternion.Euler(0, 0, offset) * rawDir;
        }

        private static void AimFirePoint(Transform firePoint, Transform player)
        {
            if (firePoint == null)
            {
                return;
            }

            Vector2 direction = ((Vector2)(player.position - firePoint.position)).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            firePoint.rotation = Quaternion.Euler(0, 0, angle);
        }

        private void FireBullet(Transform firePoint, Transform player)
        {
            if (firePoint == null)
            {
                return;
            }

            Vector2 dir = ((Vector2)(player.position - firePoint.position)).normalized;
            var damageInfo = new DamageInfo(_stats.Damage, dir * _stats.KnockbackForce);
            Bullet bullet = _bulletPool.Get();
            bullet.Activate(firePoint.position, dir, damageInfo, 1);
        }

        private void CreateTelegraph(Vector2 lockedStart, Vector2 lockedDir)
        {
            // Raycast to find bounce for the telegraph too
            float remainingRange = _railgunMaxRange;
            RaycastHit2D hit = Physics2D.Raycast(lockedStart, lockedDir, remainingRange, _stats.WallLayer);

            GameObject telegraphObj = new GameObject("RailgunTelegraph");
            _activeTelegraphs.Add(telegraphObj);
            LineRenderer telegraph = telegraphObj.AddComponent<LineRenderer>();
            telegraph.useWorldSpace = true;
            telegraph.startWidth = 0.05f;
            telegraph.endWidth = 0.05f;
            telegraph.startColor = Color.red;
            telegraph.endColor = new Color(1f, 0f, 0f, 0.3f);

            if (hit.collider != null)
            {
                // Bounce — draw 3 points: start → hit → bounce endpoint
                Vector2 hitPoint = hit.point;
                remainingRange -= hit.distance;
                Vector2 bounceDir = Vector2.Reflect(lockedDir, hit.normal);
                RaycastHit2D bounceHit = Physics2D.Raycast(hitPoint, bounceDir, remainingRange, _stats.WallLayer);
                Vector2 bounceEnd = bounceHit.collider != null ? bounceHit.point : hitPoint + bounceDir * remainingRange;

                telegraph.positionCount = 3;
                telegraph.SetPosition(0, lockedStart);
                telegraph.SetPosition(1, hitPoint);
                telegraph.SetPosition(2, bounceEnd);
            }
            else
            {
                // No bounce
                telegraph.positionCount = 2;
                telegraph.SetPosition(0, lockedStart);
                telegraph.SetPosition(1, lockedStart + lockedDir * _railgunMaxRange);
            }
        }

        private void FireRailgun(Vector2 lockedStart, Vector2 lockedDir)
        {
            Vector2 start = lockedStart;
            Vector2 dir = lockedDir;
            if (dir.sqrMagnitude < 0.001f)
            {
                dir = Vector2.up;
            }

            float remainingRange = _railgunMaxRange;
            Vector2 currentStart = start;
            Vector2 currentDir = dir;

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

            _activeBeamSegments++;
            FanManRailgunBeamSegment seg1 = _beamPool.Get();
            seg1.transform.SetParent(null);
            seg1.transform.localScale = Vector3.one;
            seg1.Setup(currentStart, end, _railgunLinger, _stats.Damage, _stats.KnockbackForce, OnBeamReleased);

            if (bounced)
            {
                _activeBeamSegments++;
                FanManRailgunBeamSegment seg2 = _beamPool.Get();
                seg2.transform.SetParent(null);
                seg2.transform.localScale = Vector3.one;
                seg2.Setup(end, bounceEnd, _railgunLinger, _stats.Damage, _stats.KnockbackForce, OnBeamReleased);
            }
        }

        private void OnBeamReleased()
        {
            _activeBeamSegments = Mathf.Max(0, _activeBeamSegments - 1);
        }
    }
}
