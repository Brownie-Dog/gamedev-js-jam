using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Enemy.Bosses
{
    public class FanManFanPushMove : MonoBehaviour, IFanManMove
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _fanPushZone;
        [SerializeField] private FanPushParticleController _particleController;
        [SerializeField] private EnemyStats _stats;
        [SerializeField] private float _startupDuration = 0.5f;
        [SerializeField] private float _pushForce = 20f;
        [SerializeField] private int _pushDamage = 1;
        [SerializeField] private float _knockbackDuration = 0.3f;
        [SerializeField] private string _startupTrigger = "FanStartup";
        [SerializeField] private string _pushBoolName = "IsFanPushing";

        private float _durationOverride = -1f;
        private float _slowMultiplierOverride = -1f;
        private Coroutine _routine;

        public bool IsComplete { get; private set; }

        public bool IsActive { get; private set; }

        public event Action OnMoveComplete;

        public void ForceStartupTrigger()
        {
            if (!string.IsNullOrEmpty(_startupTrigger))
            {
                _animator.SetTrigger(_startupTrigger);
            }
        }

        public void ForcePushBool(bool value)
        {
            if (!string.IsNullOrEmpty(_pushBoolName))
            {
                _animator.SetBool(_pushBoolName, value);
            }
        }

        private void Awake()
        {
            Assert.IsNotNull(_fanPushZone);
            Assert.IsNotNull(_stats);
            Assert.IsNotNull(_animator);
            Assert.IsNotNull(_particleController);
            _fanPushZone.SetActive(false);
        }

        private void OnDisable()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }
            if (!string.IsNullOrEmpty(_pushBoolName))
            {
                _animator.SetBool(_pushBoolName, false);
            }
            _animator.Play("Idle", 0, 0f);
            _fanPushZone.SetActive(false);
            var continuous = _fanPushZone.GetComponent<ContinuousDamageZone>();
            if (continuous != null)
            {
                continuous.Deactivate();
            }
            _particleController.Stop();
            _particleController.Clear();
        }

        public void SetDuration(float duration)
        {
            _durationOverride = duration;
        }

        public void SetSlowMultiplier(float multiplier)
        {
            _slowMultiplierOverride = multiplier;
        }

        public void Execute(Transform boss, Transform player)
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
            }
            IsComplete = false;
            IsActive = true;
            _routine = StartCoroutine(ExecuteCore(boss, player));
        }

        private IEnumerator ExecuteCore(Transform boss, Transform player)
        {
            if (!string.IsNullOrEmpty(_startupTrigger))
            {
                _animator.SetTrigger(_startupTrigger);
            }

            var zoneCollider = _fanPushZone.GetComponent<BoxCollider2D>();
            _particleController.Configure(zoneCollider);
            _particleController.PlayStartup();

            yield return new WaitForSeconds(_startupDuration);

            if (!string.IsNullOrEmpty(_pushBoolName))
            {
                _animator.SetBool(_pushBoolName, true);
            }

            float duration = _durationOverride >= 0f ? _durationOverride : 2f;
            float slowMult = _slowMultiplierOverride >= 0f ? _slowMultiplierOverride : 0.5f;

            var movement = boss.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.SetMovementSpeedMultiplier(slowMult);
            }

            _fanPushZone.SetActive(true);

            var continuous = _fanPushZone.GetComponent<ContinuousDamageZone>();
            if (continuous != null)
            {
                continuous.Configure(_pushDamage, Vector2.down * _pushForce, _knockbackDuration);
                continuous.Activate();
            }

            _particleController.PlayPush();

            yield return new WaitForSeconds(duration);

            _fanPushZone.SetActive(false);
            if (continuous != null)
            {
                continuous.Deactivate();
            }
            _particleController.Stop();

            if (!string.IsNullOrEmpty(_pushBoolName))
            {
                _animator.SetBool(_pushBoolName, false);
            }

            IsActive = false;
            IsComplete = true;
            if (OnMoveComplete != null)
            {
                OnMoveComplete.Invoke();
            }
            _routine = null;
        }
    }
}
