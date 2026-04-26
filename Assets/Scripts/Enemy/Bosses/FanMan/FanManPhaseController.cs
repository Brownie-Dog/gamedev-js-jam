using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Enemy.Bosses
{
    public class FanManPhaseController : MonoBehaviour
    {
        [SerializeField] private EnemyHealth _bossHealth;
        [SerializeField] private EnemyMovement _bossMovement;
        [SerializeField] private EnemyDetection _enemyDetection;
        [SerializeField] private FanManPhaseStats _phaseStats;
        [SerializeField] private Transform _playerTransform;

        [SerializeField] private FanManFanPushMove _fanPushMove;
        [SerializeField] private FanManGunMove _gunMove;

        public enum Phase
        {
            One,
            Two,
            Three
        }

        private Phase _currentPhase;
        private Phase _lastAppliedPhase;
        private bool _playerInRange;
        private bool _phaseForced;
        private readonly List<IFanManMove> _activeMoves = new();
        private float _cooldownTimer;

        public Phase CurrentPhase => _currentPhase;

        private void Awake()
        {
            Assert.IsNotNull(_bossHealth);
            Assert.IsNotNull(_bossMovement);
            Assert.IsNotNull(_enemyDetection);
            Assert.IsNotNull(_phaseStats);
            Assert.IsNotNull(_playerTransform);
            Assert.IsNotNull(_fanPushMove);
            Assert.IsNotNull(_gunMove);
        }

        private void Start()
        {
            ApplyPhaseSettings();
            _lastAppliedPhase = _currentPhase;
        }

        private void OnEnable()
        {
            _enemyDetection.OnPlayerDetected += HandlePlayerDetected;
            _enemyDetection.OnPlayerLost += HandlePlayerLost;
        }

        private void OnDisable()
        {
            _enemyDetection.OnPlayerDetected -= HandlePlayerDetected;
            _enemyDetection.OnPlayerLost -= HandlePlayerLost;

            foreach (var move in _activeMoves)
            {
                move.OnMoveComplete -= OnMoveCompleteHandler;
            }
            _activeMoves.Clear();
        }

        private void Update()
        {
            UpdatePhase();

            if (!_playerInRange)
            {
                return;
            }

            // Check for completed moves
            for (int i = _activeMoves.Count - 1; i >= 0; i--)
            {
                if (_activeMoves[i].IsComplete)
                {
                    _activeMoves[i].OnMoveComplete -= OnMoveCompleteHandler;
                    _activeMoves.RemoveAt(i);
                }
            }

            // If all moves finished, start cooldown
            if (_activeMoves.Count == 0 && _cooldownTimer <= 0f)
            {
                ApplyPhaseSettings();
                _cooldownTimer = GetCurrentPhaseStats().MoveCooldown;
            }

            if (_activeMoves.Count == 0)
            {
                _cooldownTimer -= Time.deltaTime;
                if (_cooldownTimer <= 0f)
                {
                    PickAndExecuteMoves();
                }
            }
        }

        private void UpdatePhase()
        {
            if (!_phaseForced)
            {
                float healthPercent = _bossHealth.HealthPercent;
                if (healthPercent > 0.66f)
                {
                    _currentPhase = Phase.One;
                }
                else if (healthPercent > 0.33f)
                {
                    _currentPhase = Phase.Two;
                }
                else
                {
                    _currentPhase = Phase.Three;
                }
            }

            if (_currentPhase != _lastAppliedPhase)
            {
                ApplyPhaseSettings();
                _lastAppliedPhase = _currentPhase;
            }
        }

        private void ApplyPhaseSettings()
        {
            var stats = GetCurrentPhaseStats();
            _bossMovement.SetMovementSpeedMultiplier(stats.MovementSpeedMultiplier);
            _fanPushMove.SetDuration(stats.FanPushDuration);
            _fanPushMove.SetSlowMultiplier(stats.FanPushSlowMultiplier);
            _gunMove.SetShotCountRange(stats.NormalGunShotMin, stats.NormalGunShotMax);
            _gunMove.SetShotInterval(stats.NormalGunShotInterval);
            _gunMove.SetAimDurationRange(stats.AimDurationMin, stats.AimDurationMax);
            _gunMove.SetRailgunStats(stats.RailgunTelegraphDuration, stats.RailgunLingerDuration, stats.RailgunMaxRange);
            _gunMove.SetGunWeights(stats.NormalGunWeight, stats.RailgunWeight);
        }

        private void PickAndExecuteMoves()
        {
            var stats = GetCurrentPhaseStats();
            float totalWeight = stats.FanPushWeight + stats.NormalGunWeight + stats.RailgunWeight;
            float roll = UnityEngine.Random.value * totalWeight;

            IFanManMove primaryMove;
            if (roll < stats.FanPushWeight)
            {
                primaryMove = _fanPushMove;
                Debug.Log("[FanMan] Picked move: FanPush");
            }
            else
            {
                primaryMove = _gunMove;
                Debug.Log("[FanMan] Picked move: Gun");
            }

            // Configure dual-hand for gun move BEFORE executing
            if (primaryMove == _gunMove)
            {
                bool dualHand = false;
                if (_currentPhase == Phase.Two && Random.value < stats.DualHandGunWeight)
                {
                    dualHand = true;
                    Debug.Log("[FanMan] Dual-hand gun mode (Phase 2)!");
                }
                else if (_currentPhase == Phase.Three && Random.value < stats.DualHandGunWeight)
                {
                    dualHand = true;
                    Debug.Log("[FanMan] Dual-hand gun mode (Phase 3)!");
                }
                _gunMove.SetUseBothHands(dualHand);
            }

            ExecuteMove(primaryMove);

            // Phase 3: Concurrent moves
            if (_currentPhase == Phase.Three && Random.value < stats.ConcurrentMoveWeight)
            {
                // Pick a secondary move that is different from primary
                IFanManMove secondaryMove = primaryMove == _fanPushMove ? _gunMove : _fanPushMove;
                Debug.Log($"[FanMan] Concurrent move: {secondaryMove.GetType().Name}");
                ExecuteMove(secondaryMove);
            }
        }

        private void ExecuteMove(IFanManMove move)
        {
            move.OnMoveComplete += OnMoveCompleteHandler;
            move.Execute(transform, _playerTransform);
            _activeMoves.Add(move);
        }

        private void OnMoveCompleteHandler()
        {
            // Cleanup handled in Update when IsComplete becomes true
        }

        private void HandlePlayerDetected(object sender, EventArgs e)
        {
            _playerInRange = true;
        }

        private void HandlePlayerLost(object sender, EventArgs e)
        {
            _playerInRange = false;
        }

        public void ForcePhase(Phase phase)
        {
            _currentPhase = phase;
            _phaseForced = true;
            ApplyPhaseSettings();
            _lastAppliedPhase = phase;
        }

        public void UnlockPhase()
        {
            _phaseForced = false;
        }

        private FanManPhaseStats.PhaseStats GetCurrentPhaseStats()
        {
            return _currentPhase switch
            {
                Phase.One => _phaseStats.Phase1,
                Phase.Two => _phaseStats.Phase2,
                Phase.Three => _phaseStats.Phase3,
                _ => _phaseStats.Phase1
            };
        }
    }
}
