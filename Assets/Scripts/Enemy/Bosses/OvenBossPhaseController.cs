using System;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Enemy.Bosses
{
    public class OvenBossPhaseController : MonoBehaviour
    {
        [SerializeField] private OvenBossArmSpawner _armSpawner;
        [SerializeField] private EnemyHealth _bossHealth;
        [SerializeField] private EnemyMovement _bossMovement;
        [SerializeField] private EnemyDetection _enemyDetection;
        [SerializeField] private OvenBossPhaseStats _phaseStats;
        [SerializeField] private Transform _playerTransform;

        [SerializeField] private OvenBossPunchMove _punchMove;
        [SerializeField] private OvenBossGrabMove _grabMove;
        [SerializeField] private OvenBossSwordMove _swordMove;

        [SerializeField] private OvenBossDoublePunchMove _doublePunchMove;
        [SerializeField] private OvenBossDoubleGrabMove _doubleGrabMove;
        [SerializeField] private OvenBossComboMove _punchSwordComboMove;
        [SerializeField] private OvenBossComboMove _swordPunchComboMove;
        [SerializeField] private OvenBossComboMove _swordGrabComboMove;

        public enum Phase
        {
            One,
            Two,
            Three
        }

        private class ArmSlot
        {
            public OvenBossArm Arm;
            public OvenBossArmController Controller;
            public IOvenBossMove CurrentMove;
            public float CooldownTimer;
            public OvenBossMoveType? PendingMoveType;

            public bool IsActive => CurrentMove != null && !CurrentMove.IsArmComplete(Arm);
        }

        private ArmSlot _leftSlot;
        private ArmSlot _rightSlot;
        private Phase _currentPhase;
        private Phase _lastAppliedPhase;
        private bool _playerInRange;
        private bool _phaseForced;
        private IOvenBossMove _currentSpecialMove;

        private void Awake()
        {
            Assert.IsNotNull(_armSpawner);
            Assert.IsNotNull(_bossHealth);
            Assert.IsNotNull(_bossMovement);
            Assert.IsNotNull(_enemyDetection);
            Assert.IsNotNull(_phaseStats);
            Assert.IsNotNull(_playerTransform);
            Assert.IsNotNull(_punchMove);
            Assert.IsNotNull(_grabMove);
            Assert.IsNotNull(_swordMove);

            _leftSlot = new ArmSlot
            {
                Arm = _armSpawner.LeftArm, Controller = _armSpawner.LeftArm.GetComponent<OvenBossArmController>()
            };
            _rightSlot = new ArmSlot
            {
                Arm = _armSpawner.RightArm, Controller = _armSpawner.RightArm.GetComponent<OvenBossArmController>()
            };
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
        }

        private void Update()
        {
            UpdatePhase();
            UpdateMovementFreeze();

            if (!_playerInRange)
            {
                return;
            }

            CheckArmCompletion(_leftSlot);
            CheckArmCompletion(_rightSlot);

            if (_currentSpecialMove != null)
            {
                if (_currentSpecialMove.IsComplete)
                {
                    OnSpecialMoveComplete();
                }

                return;
            }

            switch (_currentPhase)
            {
                case Phase.One:
                    UpdatePhaseOne();
                    break;
                case Phase.Two:
                    UpdatePhaseTwo();
                    break;
                case Phase.Three:
                    UpdatePhaseThree();
                    break;
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
            _leftSlot.Controller.SetSpeedMultiplier(stats.ArmSpeedMultiplier);
            _rightSlot.Controller.SetSpeedMultiplier(stats.ArmSpeedMultiplier);
            _bossMovement.SetMovementSpeedMultiplier(stats.MovementSpeedMultiplier);
            _punchMove.SetAimDurationRange(stats.AimDurationMin, stats.AimDurationMax);
            _grabMove.SetAimDurationRange(stats.AimDurationMin, stats.AimDurationMax);
            _swordMove.SetTelegraphDurationRange(stats.AimDurationMin, stats.AimDurationMax);
        }

        private void UpdateMovementFreeze()
        {
            bool anyLaunched = (_leftSlot.CurrentMove?.IsArmLaunched(_leftSlot.Arm) ?? false) ||
                               (_rightSlot.CurrentMove?.IsArmLaunched(_rightSlot.Arm) ?? false) ||
                               (_currentSpecialMove?.IsLaunched ?? false);

            if (anyLaunched)
            {
                _bossMovement.PauseMovement();
            }
            else
            {
                _bossMovement.ResumeMovement();
            }
        }

        private void CheckArmCompletion(ArmSlot slot)
        {
            if (slot.CurrentMove == null || !slot.CurrentMove.IsArmComplete(slot.Arm))
            {
                return;
            }

            slot.CurrentMove = null;
            var phaseStats = GetCurrentPhaseStats();
            slot.CooldownTimer = phaseStats.ArmCooldown;

            if (_currentPhase == Phase.One)
            {
                var other = slot == _leftSlot ? _rightSlot : _leftSlot;
                other.CooldownTimer = phaseStats.ArmCooldown;
            }
        }

        private void UpdatePhaseOne()
        {
            if (_leftSlot.IsActive || _rightSlot.IsActive)
            {
                return;
            }

            _leftSlot.CooldownTimer -= Time.deltaTime;
            _rightSlot.CooldownTimer -= Time.deltaTime;

            if (_leftSlot.CooldownTimer > 0f || _rightSlot.CooldownTimer > 0f)
            {
                return;
            }

            var armSlot = Random.value < 0.5f ? _leftSlot : _rightSlot;
            ExecuteSingleMove(armSlot);
        }

        private void UpdatePhaseTwo()
        {
            TryExecuteArmMove(_leftSlot);
            TryExecuteArmMove(_rightSlot);
        }

        private void UpdatePhaseThree()
        {
            bool bothIdle = !_leftSlot.IsActive && !_rightSlot.IsActive;
            bool bothCooledDown = _leftSlot.CooldownTimer <= 0f && _rightSlot.CooldownTimer <= 0f;

            if (bothIdle && bothCooledDown && HasAnySpecialMove())
            {
                var stats = GetCurrentPhaseStats();
                if (Random.value < stats.SpecialMoveWeight)
                {
                    ExecuteSpecialMove();
                    return;
                }
            }

            TryExecuteArmMove(_leftSlot);
            TryExecuteArmMove(_rightSlot);
        }

        private void TryExecuteArmMove(ArmSlot slot)
        {
            if (slot.IsActive)
            {
                return;
            }

            slot.CooldownTimer -= Time.deltaTime;
            if (slot.CooldownTimer > 0f)
            {
                return;
            }

            ExecuteSingleMove(slot);
        }

        private void ExecuteSingleMove(ArmSlot slot)
        {
            var phaseStats = GetCurrentPhaseStats();
            OvenBossMoveType moveType;

            if (slot.PendingMoveType.HasValue)
            {
                moveType = slot.PendingMoveType.Value;
            }
            else
            {
                moveType = PickMoveType(phaseStats);
            }

            var other = slot == _leftSlot ? _rightSlot : _leftSlot;
            if (moveType == OvenBossMoveType.Sword && other.CurrentMove is OvenBossSwordMove)
            {
                slot.PendingMoveType = OvenBossMoveType.Sword;
                return;
            }

            slot.PendingMoveType = null;

            IOvenBossMove move = moveType switch
            {
                OvenBossMoveType.Punch => _punchMove,
                OvenBossMoveType.Grab => _grabMove,
                OvenBossMoveType.Sword => _swordMove,
                _ => _punchMove
            };

            if (move is OvenBossPunchMove punch)
            {
                punch.SetArmOverride(slot.Arm);
                punch.SetAimDurationRange(phaseStats.AimDurationMin, phaseStats.AimDurationMax);
            }
            else if (move is OvenBossGrabMove grab)
            {
                grab.SetArmOverride(slot.Arm);
                grab.SetAimDurationRange(phaseStats.AimDurationMin, phaseStats.AimDurationMax);
            }
            else if (move is OvenBossSwordMove sword)
            {
                sword.SetArmOverride(slot.Arm);
                sword.SetTelegraphDurationRange(phaseStats.AimDurationMin, phaseStats.AimDurationMax);
            }

            move.Execute(transform, _playerTransform);
            slot.CurrentMove = move;
        }

        private OvenBossMoveType PickMoveType(OvenBossPhaseStats.PhaseStats stats)
        {
            float total = stats.PunchWeight + stats.GrabWeight + stats.SwordWeight;
            float roll = Random.value * total;

            if (roll < stats.PunchWeight)
            {
                return OvenBossMoveType.Punch;
            }

            if (roll < stats.PunchWeight + stats.GrabWeight)
            {
                return OvenBossMoveType.Grab;
            }

            return OvenBossMoveType.Sword;
        }

        private bool HasAnySpecialMove()
        {
            return _doublePunchMove != null || _doubleGrabMove != null || _punchSwordComboMove != null ||
                   _swordPunchComboMove != null || _swordGrabComboMove != null;
        }

        private void ExecuteSpecialMove()
        {
            var stats = GetCurrentPhaseStats();
            var moves = new System.Collections.Generic.List<IOvenBossMove>();
            var weights = new System.Collections.Generic.List<float>();

            if (_doublePunchMove != null)
            {
                moves.Add(_doublePunchMove);
                weights.Add(stats.DoublePunchWeight);
            }

            if (_doubleGrabMove != null)
            {
                moves.Add(_doubleGrabMove);
                weights.Add(stats.DoubleGrabWeight);
            }

            if (_punchSwordComboMove != null)
            {
                moves.Add(_punchSwordComboMove);
                weights.Add(stats.ComboWeight / 3f);
            }

            if (_swordPunchComboMove != null)
            {
                moves.Add(_swordPunchComboMove);
                weights.Add(stats.ComboWeight / 3f);
            }

            if (_swordGrabComboMove != null)
            {
                moves.Add(_swordGrabComboMove);
                weights.Add(stats.ComboWeight / 3f);
            }

            if (moves.Count == 0)
            {
                return;
            }

            float totalWeight = 0f;
            foreach (var w in weights)
            {
                totalWeight += w;
            }

            float roll = Random.value * totalWeight;

            float cumulative = 0f;
            for (int i = 0; i < moves.Count; i++)
            {
                cumulative += weights[i];
                if (roll < cumulative)
                {
                    _currentSpecialMove = moves[i];
                    _currentSpecialMove.Execute(transform, _playerTransform);
                    return;
                }
            }

            _currentSpecialMove = moves[moves.Count - 1];
            _currentSpecialMove.Execute(transform, _playerTransform);
        }

        private void OnSpecialMoveComplete()
        {
            var phaseStats = GetCurrentPhaseStats();
            _leftSlot.CooldownTimer = phaseStats.ArmCooldown;
            _rightSlot.CooldownTimer = phaseStats.ArmCooldown;
            _currentSpecialMove = null;
        }

        private OvenBossPhaseStats.PhaseStats GetCurrentPhaseStats()
        {
            return _currentPhase switch
            {
                Phase.One => _phaseStats.Phase1,
                Phase.Two => _phaseStats.Phase2,
                Phase.Three => _phaseStats.Phase3,
                _ => _phaseStats.Phase1
            };
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
    }
}