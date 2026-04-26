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
            if (_animator != null && !string.IsNullOrEmpty(_startupTrigger))
            {
                Debug.Log($"[FanPushEditor] Setting trigger '{_startupTrigger}' on '{_animator.name}'");
                _animator.SetTrigger(_startupTrigger);
            }
            else
            {
                Debug.LogWarning("[FanPushEditor] No animator or trigger name configured.");
            }
        }

        public void ForcePushBool(bool value)
        {
            if (_animator != null && !string.IsNullOrEmpty(_pushBoolName))
            {
                Debug.Log($"[FanPushEditor] Setting bool '{_pushBoolName}' = {value} on '{_animator.name}'");
                _animator.SetBool(_pushBoolName, value);
            }
            else
            {
                Debug.LogWarning("[FanPushEditor] No animator or bool name configured.");
            }
        }

        private void Awake()
        {
            Assert.IsNotNull(_fanPushZone);
            Assert.IsNotNull(_stats);
            _fanPushZone.SetActive(false);
        }

        private void OnDisable()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }
            if (!string.IsNullOrEmpty(_pushBoolName) && _animator != null)
            {
                _animator.SetBool(_pushBoolName, false);
            }
            if (_animator != null)
            {
                _animator.Play("Idle", 0, 0f);
            }
            _fanPushZone.SetActive(false);
            var continuous = _fanPushZone.GetComponent<ContinuousDamageZone>();
            if (continuous != null)
            {
                continuous.Deactivate();
            }
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
            // Startup / telegraph animation
            if (_animator != null && !string.IsNullOrEmpty(_startupTrigger))
            {
                Debug.Log($"[FanPush] Setting trigger '{_startupTrigger}' on animator '{_animator.name}'");
                _animator.SetTrigger(_startupTrigger);
            }
            else
            {
                Debug.LogWarning($"[FanPush] Animator missing or trigger name empty! animator={_animator}, trigger={_startupTrigger}");
            }
            yield return new WaitForSeconds(_startupDuration);

            if (_animator != null && !string.IsNullOrEmpty(_pushBoolName))
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

            // Activate fan zone
            _fanPushZone.SetActive(true);

            var continuous = _fanPushZone.GetComponent<ContinuousDamageZone>();
            if (continuous != null)
            {
                continuous.Configure(_pushDamage, Vector2.down * _pushForce, _knockbackDuration);
                continuous.Activate();
            }

            yield return new WaitForSeconds(duration);

            // Deactivate
            _fanPushZone.SetActive(false);
            if (continuous != null)
            {
                continuous.Deactivate();
            }

            if (!string.IsNullOrEmpty(_pushBoolName) && _animator != null)
            {
                _animator.SetBool(_pushBoolName, false);
            }

            IsActive = false;
            IsComplete = true;
            OnMoveComplete?.Invoke();
            _routine = null;
        }
    }
}
