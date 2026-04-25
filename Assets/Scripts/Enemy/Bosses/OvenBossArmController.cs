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

        private OvenBossArm _arm;
        private Transform _player;

        private void Awake()
        {
            _arm = GetComponent<OvenBossArm>();
            Assert.IsNotNull(_arm);
        }

        public void SetPlayer(Transform player)
        {
            _player = player;
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

            _arm.ExtendTo(targetSegments);

            while (_arm.IsExtending)
            {
                yield return null;
            }
        }

        public IEnumerator RetractToDefault()
        {
            _arm.RetractToDefault();

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
            float startAngle = transform.rotation.eulerAngles.z;
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
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
                SetPlayerToGrabPoint();
                yield return null;
            }

            transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
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