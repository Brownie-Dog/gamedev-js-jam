using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Enemy.Bosses
{
    public class OvenBossArmController : MonoBehaviour
    {
        [SerializeField] private float _grabPauseDuration = 0.5f;
        [SerializeField] private float _pivotSpeed = 180f;
        [SerializeField] private float _retractPauseDuration = 0.3f;
        [SerializeField] private float _handGrabOffset = 0.5f;
        [SerializeField] private float _moveStopDistance = 1.0f;
        [SerializeField] private float _baseExtensionTime = 0.3f;
        [SerializeField] private float _baseRetractionTime = 0.3f;

        private OvenBossArm _arm;
        private Rigidbody2D _rb;
        private Transform _player;
        private float _speedMultiplier = 1f;

        private void Awake()
        {
            _speedMultiplier = 1f;
            _arm = GetComponent<OvenBossArm>();
            _rb = GetComponent<Rigidbody2D>();
            Assert.IsNotNull(_arm);
            Assert.IsNotNull(_rb);
        }

        public void SetPlayer(Transform player)
        {
            _player = player;
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            _speedMultiplier = multiplier;
        }

        public IEnumerator AimPhase(float duration)
        {
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                Vector2 direction = (Vector2)(_player.position - _arm.Pivot.position).normalized;
                _arm.SetDirection(direction);
                yield return null;
            }
        }

        public IEnumerator LaunchTowardPlayer()
        {
            float distanceToPlayer = Vector2.Distance(_arm.Pivot.position, _player.position);
            int targetSegments = _arm.CalculateTargetSegments(distanceToPlayer);
            int extraSegments = Mathf.Max(targetSegments - _arm.SegmentCount, 1);
            float perSegmentDelay = _baseExtensionTime / extraSegments / _speedMultiplier;

            _arm.ExtendTo(targetSegments, perSegmentDelay);

            while (_arm.IsExtending)
            {
                yield return null;
            }
        }

        public IEnumerator RetractToDefault()
        {
            int extraSegments = Mathf.Max(_arm.SegmentCount - _arm.DefaultSegmentCount, 1);
            float perSegmentDelay = _baseRetractionTime / extraSegments / _speedMultiplier;

            _arm.RetractToDefault(perSegmentDelay);

            while (_arm.IsRetracting)
            {
                yield return null;
            }
        }

        public IEnumerator RetractToDefault(float speed)
        {
            _arm.RetractToDefault(speed);

            while (_arm.IsRetracting)
            {
                yield return null;
            }
        }

        public IEnumerator DragTowardTarget(Transform target, float moveInterval, float playerLerpSpeed,
            Func<bool> shouldStop, Action onTargetReached)
        {
            yield return GrabPausePhase(playerLerpSpeed, shouldStop);

            if (shouldStop())
            {
                yield break;
            }

            yield return PivotTowardTargetPhase(target, playerLerpSpeed, shouldStop);

            if (shouldStop())
            {
                yield break;
            }

            yield return RetractPausePhase(shouldStop);

            if (shouldStop())
            {
                yield break;
            }

            yield return MoveTowardTargetPhase(target, moveInterval, shouldStop, onTargetReached);
        }

        private IEnumerator GrabPausePhase(float playerLerpSpeed, Func<bool> shouldStop)
        {
            float timer = 0f;

            while (timer < _grabPauseDuration)
            {
                if (shouldStop())
                {
                    yield break;
                }

                timer += Time.deltaTime;
                LerpPlayerToGrabPoint(playerLerpSpeed);
                yield return null;
            }
        }

        private IEnumerator PivotTowardTargetPhase(Transform target, float playerLerpSpeed, Func<bool> shouldStop)
        {
            Vector2 directionToTarget = (Vector2)(target.position - _arm.Pivot.position).normalized;
            float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg + 90f;
            float startAngle = _rb.rotation;
            float angleDiff = Mathf.Abs(Mathf.DeltaAngle(startAngle, targetAngle));
            float pivotDuration = angleDiff / _pivotSpeed;
            float timer = 0f;

            while (timer < pivotDuration)
            {
                if (shouldStop())
                {
                    yield break;
                }

                timer += Time.deltaTime;
                float t = timer / pivotDuration;
                float angle = Mathf.LerpAngle(startAngle, targetAngle, t);
                _rb.MoveRotation(angle);
                SetPlayerToGrabPoint();
                yield return null;
            }

            _rb.MoveRotation(targetAngle);
            SetPlayerToGrabPoint();
        }

        private IEnumerator RetractPausePhase(Func<bool> shouldStop)
        {
            float timer = 0f;

            while (timer < _retractPauseDuration)
            {
                if (shouldStop())
                {
                    yield break;
                }

                timer += Time.deltaTime;
                SetPlayerToGrabPoint();
                yield return null;
            }
        }

        private IEnumerator MoveTowardTargetPhase(Transform target, float moveInterval, Func<bool> shouldStop,
            Action onTargetReached)
        {
            float moveTimer = 0f;

            while (!shouldStop())
            {
                float grabDist = Vector2.Distance(_arm.Pivot.position, GetGrabPoint());
                float targetDist = Vector2.Distance(_arm.Pivot.position, target.position);

                if (Mathf.Abs(grabDist - targetDist) <= _moveStopDistance)
                {
                    onTargetReached?.Invoke();
                    yield break;
                }

                moveTimer += Time.deltaTime;
                if (moveTimer >= moveInterval)
                {
                    if (grabDist < targetDist && _arm.CanExtend)
                    {
                        _arm.ExtendByOne();
                    }
                    else if (grabDist > targetDist && _arm.CanRetract)
                    {
                        _arm.RetractByOne();
                    }
                    else
                    {
                        yield break;
                    }

                    moveTimer = 0f;
                }

                SetPlayerToGrabPoint();
                yield return null;
            }
        }

        public IEnumerator SweepPhase(Vector2 startDirection, float sweepAngle, float speed)
        {
            _arm.SetDirection(startDirection);
            float startAngle = transform.rotation.eulerAngles.z;
            float endAngle = startAngle + sweepAngle;
            float duration = Mathf.Abs(sweepAngle) / speed;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float linearT = timer / duration;
                float t = linearT * linearT;
                float angle = startAngle + sweepAngle * t;
                _rb.MoveRotation(angle);
                yield return null;
            }

            _rb.MoveRotation(endAngle);
        }

        public IEnumerator TelegraphPhase(Vector2 direction, float duration)
        {
            _arm.SetDirection(direction);

            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }

        private Vector2 GetGrabPoint()
        {
            return (Vector2)_arm.HandPosition.position - (Vector2)transform.up * _handGrabOffset;
        }

        private void SetPlayerToGrabPoint()
        {
            if (_player != null)
            {
                _player.position = GetGrabPoint();
            }
        }

        private void LerpPlayerToGrabPoint(float lerpSpeed)
        {
            if (_player != null)
            {
                _player.position = Vector2.Lerp(_player.position, GetGrabPoint(), lerpSpeed * Time.deltaTime);
            }
        }
    }
}