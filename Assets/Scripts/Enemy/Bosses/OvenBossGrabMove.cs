using System;
using System.Collections;
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

        private Player.DamageDealer _damageDealer;
        private PlayerMovementController _playerMovement;
        private Rigidbody2D _playerRb;
        private GrabHand _grabHand;
        private Coroutine _grabRoutine;
        private bool _grabBroken;
        private Transform _player;

        public bool IsComplete { get; private set; }
        public event Action OnMoveComplete;

        private void Awake()
        {
            Assert.IsNotNull(_armSpawner);
            Assert.IsNotNull(_openClawHandPrefab);
            Assert.IsNotNull(_stats);
            Assert.IsNotNull(_dragTarget);
            Assert.IsNotNull(_enemyMovement);
        }

        public void Execute(Transform boss, Transform player)
        {
            IsComplete = false;
            _enemyMovement.PauseMovement();

            if (_grabRoutine != null)
            {
                StopCoroutine(_grabRoutine);
            }

            _grabRoutine = StartCoroutine(GrabRoutine(boss, player));
        }

        private IEnumerator GrabRoutine(Transform boss, Transform player)
        {
            _player = player;
            _playerMovement = player.GetComponent<PlayerMovementController>();
            _playerRb = player.GetComponent<Rigidbody2D>();
            _grabBroken = false;

            OvenBossArm arm = Random.value < 0.5f ? _armSpawner.LeftArm : _armSpawner.RightArm;
            var armController = arm.GetComponent<OvenBossArmController>();
            armController.SetPlayer(player);

            arm.SwapHand(_openClawHandPrefab);

            _damageDealer = arm.GetHandComponent<Player.DamageDealer>();
            Assert.IsNotNull(_damageDealer, "OpenClawHand prefab must have a DamageDealer component");

            _grabHand = arm.GetHandComponent<GrabHand>();
            Assert.IsNotNull(_grabHand, "OpenClawHand prefab must have a GrabHand component");

            yield return armController.AimPhase(Random.Range(_aimDurationMin, _aimDurationMax));

            var damageInfo = new DamageInfo(_grabDamage, Vector2.zero);
            _damageDealer.Activate(damageInfo);
            _grabHand.Activate();

            yield return armController.LaunchTowardPlayer();

            _damageDealer.Deactivate();

            float detectionTimer = 0f;
            while (detectionTimer < _grabDetectionWindow && !_grabHand.IsPlayerInReach)
            {
                detectionTimer += Time.deltaTime;
                yield return null;
            }

            if (!_grabHand.IsPlayerInReach)
            {
                _grabHand.Deactivate();
                _damageDealer = null;
                _grabHand = null;
                _player = null;

                yield return armController.RetractToDefault();

                arm.SwapToDefaultHand();
                IsComplete = true;
                _enemyMovement.ResumeMovement();
                OnMoveComplete?.Invoke();
                _grabRoutine = null;
                yield break;
            }

            _playerMovement.enabled = false;
            _playerRb.linearVelocity = Vector2.zero;

            _grabHand.OnGrabBroken += OnGrabBroken;

            yield return armController.DragTowardTarget(_dragTarget, _dragMoveInterval, _playerLerpSpeed,
                () => _grabBroken, OnTargetReached
            );

            _playerMovement.enabled = true;
            _grabHand.OnGrabBroken -= OnGrabBroken;
            _grabHand.Deactivate();

            _damageDealer = null;
            _grabHand = null;
            _player = null;

            yield return armController.RetractToDefault();

            arm.SwapToDefaultHand();
            IsComplete = true;
            _enemyMovement.ResumeMovement();
            OnMoveComplete?.Invoke();
            _grabRoutine = null;
        }

        private void OnGrabBroken(object sender, EventArgs e)
        {
            _grabBroken = true;
        }

        private void OnTargetReached()
        {
            _grabBroken = true;
        }
    }
}
