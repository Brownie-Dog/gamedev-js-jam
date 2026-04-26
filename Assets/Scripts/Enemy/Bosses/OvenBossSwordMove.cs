using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Enemy.Bosses
{
    public class OvenBossSwordMove : MonoBehaviour, IOvenBossMove
    {
        [SerializeField] private OvenBossArmSpawner _armSpawner;
        [SerializeField] private GameObject _swordHandPrefab;
        [SerializeField] private EnemyStats _stats;
        [SerializeField] private EnemyMovement _enemyMovement;
        [SerializeField] private float _telegraphDurationMin = 2f;
        [SerializeField] private float _telegraphDurationMax = 5f;
        [SerializeField] private float _swingSpeed = 180f;
        [SerializeField] private float _sweepAngle = 135f;
        [SerializeField] private int _swordDamage = 3;
        [SerializeField] private float _swordKnockbackForce = 30f;
        [SerializeField] private float _swordKnockbackDuration = 0.5f;
        [SerializeField] private float _hitPauseDuration = 0.2f;

        private readonly Dictionary<OvenBossArm, Coroutine> _routines = new Dictionary<OvenBossArm, Coroutine>();
        private readonly Dictionary<OvenBossArm, bool> _armComplete = new Dictionary<OvenBossArm, bool>();
        private readonly Dictionary<OvenBossArm, bool> _armLaunched = new Dictionary<OvenBossArm, bool>();
        private readonly Dictionary<OvenBossArm, bool> _armAttacking = new Dictionary<OvenBossArm, bool>();

        private OvenBossArm _forcedArm;
        private Func<bool> _attackGate;
        private float _telegraphMinOverride = -1f;
        private float _telegraphMaxOverride = -1f;

        public bool IsComplete => !_armComplete.ContainsValue(false);
        public bool IsLaunched => _armLaunched.ContainsValue(true);
        public bool IsAttacking => _armAttacking.ContainsValue(true);
        public event Action OnMoveComplete;

        private static readonly Vector2 West = Vector2.left;
        private static readonly Vector2 East = Vector2.right;

        private void Awake()
        {
            Assert.IsNotNull(_armSpawner);
            Assert.IsNotNull(_swordHandPrefab);
            Assert.IsNotNull(_stats);
            Assert.IsNotNull(_enemyMovement);
        }

        public bool IsArmComplete(OvenBossArm arm) => _armComplete.GetValueOrDefault(arm, true);
        public bool IsArmLaunched(OvenBossArm arm) => _armLaunched.GetValueOrDefault(arm, false);
        public bool IsArmAttacking(OvenBossArm arm) => _armAttacking.GetValueOrDefault(arm, false);

        public void SetArmOverride(OvenBossArm arm)
        {
            _forcedArm = arm;
        }

        public void SetAttackGate(Func<bool> gate)
        {
            _attackGate = gate;
        }

        public void SetTelegraphDurationRange(float min, float max)
        {
            _telegraphMinOverride = min;
            _telegraphMaxOverride = max;
        }

        private float GetTelegraphMin() => _telegraphMinOverride >= 0f ? _telegraphMinOverride : _telegraphDurationMin;
        private float GetTelegraphMax() => _telegraphMaxOverride >= 0f ? _telegraphMaxOverride : _telegraphDurationMax;

        public void Execute(Transform boss, Transform player)
        {
            bool isLeftArm;
            OvenBossArm arm;

            if (_forcedArm != null)
            {
                arm = _forcedArm;
                isLeftArm = arm == _armSpawner.LeftArm;
                _forcedArm = null;
            }
            else
            {
                isLeftArm = Random.value < 0.5f;
                arm = isLeftArm ? _armSpawner.LeftArm : _armSpawner.RightArm;
            }

            _armComplete[arm] = false;
            _armLaunched[arm] = false;
            _armAttacking[arm] = false;

            if (_routines.TryGetValue(arm, out var existing))
            {
                if (existing != null) StopCoroutine(existing);
            }

            _routines[arm] = StartCoroutine(WrappedCore(arm, player, isLeftArm));
        }

        private IEnumerator WrappedCore(OvenBossArm arm, Transform player, bool isLeftArm)
        {
            _enemyMovement.PauseMovement();
            yield return ExecuteCore(arm, player, isLeftArm);
            _enemyMovement.ResumeMovement();
            _armComplete[arm] = true;
            OnMoveComplete?.Invoke();
            _routines.Remove(arm);
        }

        public IEnumerator ExecuteCore(OvenBossArm arm, Transform player)
        {
            bool isLeftArm = arm == _armSpawner.LeftArm;
            yield return ExecuteCore(arm, player, isLeftArm);
        }

        private IEnumerator ExecuteCore(OvenBossArm arm, Transform player, bool isLeftArm)
        {
            arm.SwapToDefaultHand();

            var armController = arm.GetComponent<OvenBossArmController>();
            armController.SetPlayer(player);

            Vector2 startDirection = isLeftArm ? West : East;
            float sweepAngle = isLeftArm ? _sweepAngle : -_sweepAngle;
            Vector2 knockback = isLeftArm ? East : West;

            yield return armController.PivotToDirection(startDirection);

            arm.SwapHand(_swordHandPrefab);

            var damageDealer = arm.GetHandComponent<Player.DamageDealer>();
            Assert.IsNotNull(damageDealer, "SwordHand prefab must have a DamageDealer component");

            var damageInfo = new DamageInfo(_swordDamage, knockback * _swordKnockbackForce, _swordKnockbackDuration);

            yield return armController.TelegraphPhase(startDirection, Random.Range(GetTelegraphMin(), GetTelegraphMax()));

            if (_attackGate != null)
            {
                yield return new WaitUntil(() => _attackGate());
                _attackGate = null;
            }

            _armLaunched[arm] = true;
            _armAttacking[arm] = true;
            damageDealer.Activate(damageInfo);

            yield return armController.SweepPhase(startDirection, sweepAngle, _swingSpeed);

            damageDealer.Deactivate();

            yield return new WaitForSeconds(_hitPauseDuration);
            _armAttacking[arm] = false;

            arm.SwapToDefaultHand();
            _armLaunched[arm] = false;
        }
    }
}
