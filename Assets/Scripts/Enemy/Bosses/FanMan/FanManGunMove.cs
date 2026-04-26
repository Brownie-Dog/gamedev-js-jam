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

        [Header("Hand Pivots")]
        [SerializeField] private Transform _leftHandPivot;
        [SerializeField] private Transform _rightHandPivot;

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
        private float _normalGunWeight = 0.5f;
        private float _railgunWeight = 0f;
        private bool _useBothHands;
        private const float PIVOT_SPEED = 360f;

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
            Assert.IsNotNull(_leftNormalGun1);
            Assert.IsNotNull(_leftNormalGun1FirePoint);
            Assert.IsNotNull(_leftNormalGun2);
            Assert.IsNotNull(_leftNormalGun2FirePoint);
            Assert.IsNotNull(_leftRailgun);
            Assert.IsNotNull(_leftRailgunFirePoint);
            Assert.IsNotNull(_rightNormalGun1);
            Assert.IsNotNull(_rightNormalGun1FirePoint);
            Assert.IsNotNull(_rightNormalGun2);
            Assert.IsNotNull(_rightNormalGun2FirePoint);
            Assert.IsNotNull(_rightRailgun);
            Assert.IsNotNull(_rightRailgunFirePoint);
            Assert.IsNotNull(_leftHandPivot);
            Assert.IsNotNull(_rightHandPivot);

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
            _leftNormalGun1.SetActive(false);
            _leftNormalGun2.SetActive(false);
            _leftRailgun.SetActive(false);
            _rightNormalGun1.SetActive(false);
            _rightNormalGun2.SetActive(false);
            _rightRailgun.SetActive(false);
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

        public void SetMultiShotConfig(int min, int max, float interval, float varianceDeg)
        {
            _railgunMultiShotMin = min;
            _railgunMultiShotMax = max;
            _railgunTelegraphInterval = interval;
            _railgunAimVarianceDeg = varianceDeg;
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
            if (OnMoveComplete != null)
            {
                OnMoveComplete.Invoke();
            }
            _routine = null;
        }

        private void ActivateGun(GameObject gun)
        {
            gun.SetActive(true);
            _activeGuns.Add(gun);
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
            Transform leftPivot = _leftHandPivot;
            Transform rightPivot = _rightHandPivot;

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

            Transform activeFire = dualHand ? leftFire : (_activeGuns.Count > 0 && _activeGuns[0] == leftGun ? leftFire : rightFire);
            Transform activePivot = dualHand ? leftPivot : (_activeGuns.Count > 0 && _activeGuns[0] == leftGun ? leftPivot : rightPivot);
            Transform secondaryFire = dualHand ? rightFire : null;
            Transform secondaryPivot = dualHand ? rightPivot : null;

            if (!dualHand && activeFire == null)
            {
                activeFire = leftFire != null ? leftFire : rightFire;
                activePivot = leftFire != null ? leftPivot : rightPivot;
            }

            // Aim phase
            float aimDuration = Random.Range(_aimMin, _aimMax);
            float aimTimer = 0f;
            while (aimTimer < aimDuration)
            {
                aimTimer += Time.deltaTime;
                AimHandPivot(activePivot, player);
                if (dualHand)
                {
                    AimHandPivot(secondaryPivot, player);
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
                        FireBullet(activeFire, player);
                        FireBullet(secondaryFire, player);
                        yield return TrackDuringInterval(activePivot, secondaryPivot, player, _shotInterval);
                    }
                }
                else
                {
                    // Alternating fire
                    bool leftFirst = Random.value < 0.5f;
                    Transform firstFire = leftFirst ? leftFire : rightFire;
                    Transform secondFire = leftFirst ? rightFire : leftFire;
                    Transform firstPivot = leftFirst ? leftPivot : rightPivot;
                    Transform secondPivot = leftFirst ? rightPivot : leftPivot;
                    for (int i = 0; i < shots; i++)
                    {
                        FireBullet(firstFire, player);
                        yield return TrackDuringInterval(firstPivot, secondPivot, player, _shotInterval);
                        FireBullet(secondFire, player);
                        yield return TrackDuringInterval(firstPivot, secondPivot, player, _shotInterval);
                    }
                }
            }
            else
            {
                // Single hand
                for (int i = 0; i < shots; i++)
                {
                    FireBullet(activeFire, player);
                    yield return TrackDuringInterval(activePivot, null, player, _shotInterval);
                }
            }
        }

        private IEnumerator ExecuteRailgun(Transform player, bool dualHand)
        {
            Transform activeFire;
            Transform secondaryFire = null;
            Transform activePivot;
            Transform secondaryPivot = null;

            if (dualHand)
            {
                ActivateGun(_leftRailgun);
                ActivateGun(_rightRailgun);
                activeFire = _leftRailgunFirePoint;
                secondaryFire = _rightRailgunFirePoint;
                activePivot = _leftHandPivot;
                secondaryPivot = _rightHandPivot;
            }
            else
            {
                bool useLeft = Random.value < 0.5f;
                ActivateGun(useLeft ? _leftRailgun : _rightRailgun);
                activeFire = useLeft ? _leftRailgunFirePoint : _rightRailgunFirePoint;
                activePivot = useLeft ? _leftHandPivot : _rightHandPivot;
            }

            int shotCount = Mathf.Max(1, Random.Range(_railgunMultiShotMin, _railgunMultiShotMax + 1));

            // Initial aim phase
            float aimDuration = Random.Range(_aimMin, _aimMax);
            yield return AimPhaseCoroutine(activePivot, secondaryPivot, player, aimDuration);

            // Collect locked shots per hand
            var activeShots = new List<(Vector2 start, Vector2 dir)>();
            var secondaryShots = new List<(Vector2 start, Vector2 dir)>();

            for (int shot = 0; shot < shotCount; shot++)
            {
                // Lock aim for this shot
                if (activeFire != null)
                {
                    Vector2 start = activeFire.position;
                    Vector2 dir = CalculateAimDirection(activeFire, player, shot == 0);
                    activeShots.Add((start, dir));
                    LockPivotAtDirection(activePivot, dir);
                    CreateTelegraph(start, dir);
                }
                if (secondaryFire != null)
                {
                    Vector2 start = secondaryFire.position;
                    Vector2 dir = CalculateAimDirection(secondaryFire, player, shot == 0);
                    secondaryShots.Add((start, dir));
                    LockPivotAtDirection(secondaryPivot, dir);
                    CreateTelegraph(start, dir);
                }

                yield return new WaitForSeconds(_railgunTelegraph);

                // Re-aim and wait interval before next telegraph (if not last shot)
                if (shot < shotCount - 1)
                {
                    yield return AimPhaseCoroutine(activePivot, secondaryPivot, player, _railgunTelegraphInterval);
                }
            }

            // Clear all telegraphs before firing
            ClearTelegraphs();

            // Fire all shots in sequence with quick pivot + tiny delay
            for (int i = 0; i < activeShots.Count; i++)
            {
                yield return QuickPivotToDirection(activePivot, activeShots[i].dir);
                FireRailgun(activeShots[i].start, activeShots[i].dir);
                if (dualHand && i < secondaryShots.Count)
                {
                    yield return QuickPivotToDirection(secondaryPivot, secondaryShots[i].dir);
                    FireRailgun(secondaryShots[i].start, secondaryShots[i].dir);
                }

                yield return new WaitForSeconds(0.05f);
            }
        }

        private IEnumerator AimPhaseCoroutine(Transform pivotA, Transform pivotB, Transform player, float duration)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                AimHandPivot(pivotA, player);
                AimHandPivot(pivotB, player);
                yield return null;
            }
        }

        private IEnumerator TrackDuringInterval(Transform pivotA, Transform pivotB, Transform player, float duration)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                AimHandPivot(pivotA, player);
                AimHandPivot(pivotB, player);
                yield return null;
            }
        }

        private static void AimHandPivot(Transform pivot, Transform player)
        {
            if (pivot == null || player == null)
            {
                return;
            }

            Vector2 direction = ((Vector2)(player.position - pivot.position)).normalized;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
            float currentAngle = pivot.eulerAngles.z;
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, PIVOT_SPEED * Time.deltaTime);
            pivot.rotation = Quaternion.Euler(0, 0, newAngle);
        }

        private static void LockPivotAtDirection(Transform pivot, Vector2 direction)
        {
            if (pivot == null)
            {
                return;
            }

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
            pivot.rotation = Quaternion.Euler(0, 0, angle);
        }

        private static IEnumerator QuickPivotToDirection(Transform pivot, Vector2 direction)
        {
            if (pivot == null)
            {
                yield break;
            }

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
            while (true)
            {
                float currentAngle = pivot.eulerAngles.z;
                float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, PIVOT_SPEED * Time.deltaTime);
                pivot.rotation = Quaternion.Euler(0, 0, newAngle);
                if (Mathf.Abs(Mathf.DeltaAngle(newAngle, targetAngle)) < 2f)
                {
                    break;
                }
                yield return null;
            }
        }

        private Vector2 CalculateAimDirection(Transform firePoint, Transform player, bool isFirstShot)
        {
            Vector2 rawDir = ((Vector2)(player.position - firePoint.position)).normalized;

            // First shot is always accurate; follow-up shots always get variance
            if (isFirstShot || _railgunAimVarianceDeg <= 0f)
            {
                return rawDir;
            }

            // Guaranteed random angular offset for follow-up shots
            float offset = Random.Range(-_railgunAimVarianceDeg, _railgunAimVarianceDeg);
            return Quaternion.Euler(0, 0, offset) * rawDir;
        }

        private void FireBullet(Transform firePoint, Transform player)
        {
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
