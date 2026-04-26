using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Enemy.Bosses
{
    public class OvenBossPunchMove : MonoBehaviour, IOvenBossMove
    {
        [SerializeField] private OvenBossArmSpawner _armSpawner;
        [SerializeField] private GameObject _closedClawHandPrefab;
        [SerializeField] private EnemyStats _stats;
        [SerializeField] private EnemyMovement _bossMovement;
        [SerializeField] private float _aimDurationMin = 2f;
        [SerializeField] private float _aimDurationMax = 5f;
        [SerializeField] private float _hitPauseDuration = 0.2f;

        private readonly Dictionary<OvenBossArm, Coroutine> _routines = new Dictionary<OvenBossArm, Coroutine>();
        private readonly Dictionary<OvenBossArm, bool> _armComplete = new Dictionary<OvenBossArm, bool>();
        private readonly Dictionary<OvenBossArm, bool> _armLaunched = new Dictionary<OvenBossArm, bool>();
        private readonly Dictionary<OvenBossArm, bool> _armAttacking = new Dictionary<OvenBossArm, bool>();

        private OvenBossArm _forcedArm;
        private Func<bool> _attackGate;
        private float _aimMinOverride = -1f;
        private float _aimMaxOverride = -1f;

        public bool IsComplete => !_armComplete.ContainsValue(false);
        public bool IsLaunched => _armLaunched.ContainsValue(true);
        public bool IsAttacking => _armAttacking.ContainsValue(true);
        public event Action OnMoveComplete;

        private void Awake()
        {
            Assert.IsNotNull(_armSpawner);
            Assert.IsNotNull(_closedClawHandPrefab);
            Assert.IsNotNull(_stats);
            Assert.IsNotNull(_bossMovement);
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

        public void SetAimDurationRange(float min, float max)
        {
            _aimMinOverride = min;
            _aimMaxOverride = max;
        }

        private float GetAimMin() => _aimMinOverride >= 0f ? _aimMinOverride : _aimDurationMin;
        private float GetAimMax() => _aimMaxOverride >= 0f ? _aimMaxOverride : _aimDurationMax;

        public void Execute(Transform boss, Transform player)
        {
            OvenBossArm arm = _forcedArm ?? (Random.value < 0.5f ? _armSpawner.LeftArm : _armSpawner.RightArm);
            _forcedArm = null;

            _armComplete[arm] = false;
            _armLaunched[arm] = false;
            _armAttacking[arm] = false;

            if (_routines.TryGetValue(arm, out var existing))
            {
                if (existing != null) StopCoroutine(existing);
            }

            _routines[arm] = StartCoroutine(WrappedCore(arm, player));
        }

        private IEnumerator WrappedCore(OvenBossArm arm, Transform player)
        {
            _bossMovement.PauseMovement();
            yield return ExecuteCore(arm, player);
            _bossMovement.ResumeMovement();
            _armComplete[arm] = true;
            OnMoveComplete?.Invoke();
            _routines.Remove(arm);
        }

        public IEnumerator ExecuteCore(OvenBossArm arm, Transform player)
        {
            arm.SwapToDefaultHand();

            var armController = arm.GetComponent<OvenBossArmController>();
            armController.SetPlayer(player);

            arm.SwapHand(_closedClawHandPrefab);
            var damageDealer = arm.GetHandComponent<Player.DamageDealer>();
            Assert.IsNotNull(damageDealer, "ClosedClawHand prefab must have a DamageDealer component");

            var damageInfo = new DamageInfo(_stats.Damage, new Vector2(_stats.KnockbackForce, 0f));

            yield return armController.AimPhase(Random.Range(GetAimMin(), GetAimMax()));

            if (_attackGate != null)
            {
                yield return new WaitUntil(() => _attackGate());
                _attackGate = null;
            }

            _armLaunched[arm] = true;
            _armAttacking[arm] = true;
            damageDealer.Activate(damageInfo, false);

            yield return armController.LaunchTowardPlayer();
            yield return new WaitForSeconds(_hitPauseDuration);

            damageDealer.Deactivate();

            _armAttacking[arm] = false;
            yield return armController.RetractToDefault();

            arm.SwapToDefaultHand();
            _armLaunched[arm] = false;
        }
    }
}
