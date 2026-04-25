using System;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Enemy.Bosses
{
    public class OvenFlameSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private BoxCollider2D _targetArea;
        [SerializeField] private FlameProjectile _projectilePrefab;
        [SerializeField] private FlameFireTile _fireTilePrefab;
        [SerializeField] private OvenBossPhaseStats _phaseStats;
        [SerializeField] private OvenBossPhaseController _phaseController;
        [SerializeField] private int _poolSize = 20;

        private float _cooldownTimer;
        private bool _isBursting;
        private int _shotsRemaining;
        private float _shotTimer;
        private OvenBossPhaseController.Phase _currentPhase;

        private MonoBehaviourPool<FlameProjectile> _projectilePool;
        private MonoBehaviourPool<FlameFireTile> _fireTilePool;

        private void Awake()
        {
            Assert.IsNotNull(_spawnPoint);
            Assert.IsNotNull(_targetArea);
            Assert.IsNotNull(_projectilePrefab);
            Assert.IsNotNull(_fireTilePrefab);
            Assert.IsNotNull(_phaseStats);
            Assert.IsNotNull(_phaseController);

            _projectilePool = new MonoBehaviourPool<FlameProjectile>(_projectilePrefab, _poolSize, _poolSize, transform,
                OnProjectileCreated
            );
            _fireTilePool =
                new MonoBehaviourPool<FlameFireTile>(_fireTilePrefab, _poolSize, _poolSize, transform, OnFireTileCreated
                );
        }

        private void Start()
        {
            ResetCooldown();
        }

        private void OnProjectileCreated(FlameProjectile projectile)
        {
            projectile.SetPool(_projectilePool);
            projectile.SetSpawnFireTileAction(SpawnFireTile);
        }

        private void OnFireTileCreated(FlameFireTile fireTile)
        {
            fireTile.SetPool(_fireTilePool);
        }

        private void Update()
        {
            UpdatePhase();

            if (_isBursting)
            {
                _shotTimer -= Time.deltaTime;
                if (_shotTimer <= 0f)
                {
                    FireShot();
                    _shotsRemaining--;

                    if (_shotsRemaining <= 0)
                    {
                        _isBursting = false;
                        ResetCooldown();
                    }
                    else
                    {
                        _shotTimer = GetPhaseStats().FlameShotInterval;
                    }
                }
            }
            else
            {
                _cooldownTimer -= Time.deltaTime;
                if (_cooldownTimer <= 0f)
                {
                    StartBurst();
                }
            }
        }

        private void UpdatePhase()
        {
            _currentPhase = _phaseController.CurrentPhase;
        }

        private void StartBurst()
        {
            _isBursting = true;
            _shotsRemaining = GetPhaseStats().FlameBurstCount;
            _shotTimer = 0f;
        }

        private void FireShot()
        {
            Vector2 target = GetRandomPointInArea();
            FlameProjectile projectile = _projectilePool.Get();
            projectile.transform.position = _spawnPoint.position;
            projectile.transform.rotation = Quaternion.identity;
            projectile.SetTarget(target);
        }

        private void SpawnFireTile(Vector2 position)
        {
            FlameFireTile fireTile = _fireTilePool.Get();
            fireTile.transform.position = position;
            fireTile.transform.rotation = Quaternion.identity;
            fireTile.ResetLifetime();
        }

        private Vector2 GetRandomPointInArea()
        {
            Bounds bounds = _targetArea.bounds;
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);
            return new Vector2(x, y);
        }

        private void ResetCooldown()
        {
            _cooldownTimer = GetPhaseStats().FlameBurstCooldown;
        }

        private OvenBossPhaseStats.PhaseStats GetPhaseStats()
        {
            return _currentPhase switch
            {
                OvenBossPhaseController.Phase.One => _phaseStats.Phase1,
                OvenBossPhaseController.Phase.Two => _phaseStats.Phase2,
                OvenBossPhaseController.Phase.Three => _phaseStats.Phase3,
                _ => _phaseStats.Phase1
            };
        }
    }
}