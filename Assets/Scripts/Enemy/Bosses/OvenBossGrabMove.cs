using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Enemy.Bosses
{
    public class OvenBossGrabMove : MonoBehaviour, IOvenBossMove
    {
        [SerializeField] private OvenBossArmSpawner _armSpawner;
        [SerializeField] private GameObject _openClawHandPrefab;
        [SerializeField] private EnemyStats _stats;
        [SerializeField] private Transform _dragTarget;
        [SerializeField] private EnemyMovement _enemyMovement;
        [SerializeField] private float _aimDurationMin = 2f;
        [SerializeField] private float _aimDurationMax = 5f;
        [SerializeField] private float _dragMoveInterval = 0.05f;
        [SerializeField] private float _playerLerpSpeed = 10f;
        [SerializeField] private float _grabDetectionWindow = 0.5f;
        [SerializeField] private int _grabDamage = 1;

        private readonly Dictionary<OvenBossArm, Coroutine> _routines = new Dictionary<OvenBossArm, Coroutine>();
        private readonly Dictionary<OvenBossArm, bool> _armComplete = new Dictionary<OvenBossArm, bool>();
        private readonly Dictionary<OvenBossArm, bool> _armLaunched = new Dictionary<OvenBossArm, bool>();
        private readonly Dictionary<OvenBossArm, bool> _armAttacking = new Dictionary<OvenBossArm, bool>();

        private OvenBossArm _forcedArm;
        private Func<bool> _attackGate;
        private bool _anyGrabActive;

        public bool IsComplete => !_armComplete.ContainsValue(false);
        public bool IsLaunched => _armLaunched.ContainsValue(true);
        public bool IsAttacking => _armAttacking.ContainsValue(true);
        public event Action OnMoveComplete;

        private void Awake()
        {
            Assert.IsNotNull(_armSpawner);
            Assert.IsNotNull(_openClawHandPrefab);
            Assert.IsNotNull(_stats);
            Assert.IsNotNull(_dragTarget);
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

        public void ResetGrabState()
        {
            _anyGrabActive = false;
        }

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
            _enemyMovement.PauseMovement();
            yield return ExecuteCore(arm, player);
            _enemyMovement.ResumeMovement();
            _armComplete[arm] = true;
            OnMoveComplete?.Invoke();
            _routines.Remove(arm);
        }

        public IEnumerator ExecuteCore(OvenBossArm arm, Transform player)
        {
            var playerMovement = player.GetComponent<PlayerMovementController>();
            var playerRb = player.GetComponent<Rigidbody2D>();

            var armController = arm.GetComponent<OvenBossArmController>();
            armController.SetPlayer(player);

            arm.SwapHand(_openClawHandPrefab);

            var damageDealer = arm.GetHandComponent<Player.DamageDealer>();
            Assert.IsNotNull(damageDealer, "OpenClawHand prefab must have a DamageDealer component");

            var grabHand = arm.GetHandComponent<GrabHand>();
            Assert.IsNotNull(grabHand, "OpenClawHand prefab must have a GrabHand component");

            yield return armController.AimPhase(Random.Range(_aimDurationMin, _aimDurationMax));

            if (_attackGate != null)
            {
                yield return new WaitUntil(() => _attackGate());
                _attackGate = null;
            }

            _armLaunched[arm] = true;
            _armAttacking[arm] = true;

            var damageInfo = new DamageInfo(_grabDamage, Vector2.zero);
            damageDealer.Activate(damageInfo);
            grabHand.Activate();

            yield return armController.LaunchTowardPlayer();

            damageDealer.Deactivate();

            float detectionTimer = 0f;
            bool grabbed = false;
            while (detectionTimer < _grabDetectionWindow && !grabbed)
            {
                detectionTimer += Time.deltaTime;
                grabbed = grabHand.IsPlayerInReach;
                yield return null;
            }

            if (!grabbed)
            {
                grabHand.Deactivate();
                _armAttacking[arm] = false;
                yield return armController.RetractToDefault();
                arm.SwapToDefaultHand();
                _armLaunched[arm] = false;
                yield break;
            }

            if (_anyGrabActive)
            {
                grabHand.Deactivate();
                _armAttacking[arm] = false;
                yield return armController.RetractToDefault();
                arm.SwapToDefaultHand();
                _armLaunched[arm] = false;
                yield break;
            }

            _anyGrabActive = true;

            playerMovement.enabled = false;
            playerRb.linearVelocity = Vector2.zero;

            bool grabBroken = false;
            EventHandler onGrabBroken = (_, _) => grabBroken = true;
            grabHand.OnGrabBroken += onGrabBroken;

            yield return armController.DragTowardTarget(_dragTarget, _dragMoveInterval, _playerLerpSpeed,
                () => grabBroken, () => grabBroken = true);

            playerMovement.enabled = true;
            grabHand.OnGrabBroken -= onGrabBroken;
            grabHand.Deactivate();

            _armAttacking[arm] = false;
            yield return armController.RetractToDefault();
            arm.SwapToDefaultHand();
            _armLaunched[arm] = false;
            _anyGrabActive = false;
        }
    }
}
