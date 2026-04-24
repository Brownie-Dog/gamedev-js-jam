using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Enemy.Bosses
{
    public class OvenBossAI : MonoBehaviour
    {
        [SerializeField] private OvenBossAttack _bossAttack;
        [SerializeField] private EnemyDetection _enemyDetection;
        [SerializeField] private EnemyStats _stats;

        private enum BossState
        {
            Idle,
            Attacking,
            Cooldown
        }

        private BossState _state = BossState.Idle;
        private float _cooldownTimer;
        private bool _playerInRange;

        private void Awake()
        {
            Assert.IsNotNull(_bossAttack);
            Assert.IsNotNull(_enemyDetection);
            Assert.IsNotNull(_stats);
            Debug.Log("[OvenBossAI] Awake - bossAttack, enemyDetection, stats all valid");
        }

        private void OnEnable()
        {
            _enemyDetection.OnPlayerDetected += HandlePlayerDetected;
            _enemyDetection.OnPlayerLost += HandlePlayerLost;
            Debug.Log("[OvenBossAI] OnEnable - subscribed to detection events");
        }

        private void OnDisable()
        {
            _enemyDetection.OnPlayerDetected -= HandlePlayerDetected;
            _enemyDetection.OnPlayerLost -= HandlePlayerLost;
        }

        private void Update()
        {
            switch (_state)
            {
                case BossState.Idle:
                    if (_playerInRange)
                    {
                        Debug.Log("[OvenBossAI] Player in range, executing attack");
                        _bossAttack.ExecuteAttack();
                        _state = BossState.Attacking;
                    }
                    break;

                case BossState.Attacking:
                    if (_bossAttack.IsMoveComplete)
                    {
                        Debug.Log("[OvenBossAI] Move complete, entering cooldown");
                        _cooldownTimer = _stats.AttackCooldown;
                        _state = BossState.Cooldown;
                    }
                    break;

                case BossState.Cooldown:
                    _cooldownTimer -= Time.deltaTime;
                    if (_cooldownTimer <= 0f)
                    {
                        Debug.Log("[OvenBossAI] Cooldown finished, returning to idle");
                        _state = BossState.Idle;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandlePlayerDetected(object sender, System.EventArgs e)
        {
            _playerInRange = true;
            Debug.Log("[OvenBossAI] Player detected");
        }

        private void HandlePlayerLost(object sender, System.EventArgs e)
        {
            _playerInRange = false;
            Debug.Log("[OvenBossAI] Player lost");
        }
    }
}