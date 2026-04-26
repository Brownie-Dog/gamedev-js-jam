using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace EndlessMode
{
    public class EndlessSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EndlessModeManager _manager;
        [SerializeField] private GameObject[] _enemyPrefabs;
        
        [Header("Spawn Zone")]
        [Tooltip("Drag the invisible 'SpawnBounds' object here.")]
        [SerializeField] private Collider2D _spawnArea; 

        [Header("Settings")]
        [SerializeField] private float _timeBetweenSpawns = 1.0f;

        [Header("Safe Spawn Settings")]
        [SerializeField] private Transform _playerTransform; 
        [SerializeField] private float _minSpawnDistance = 5f;
        
        private List<GameObject> _activeEnemies = new List<GameObject>();
        private bool _isSpawning = false;

        private void Awake()
        {
            Assert.IsNotNull(_manager);
            Assert.IsNotNull(_enemyPrefabs);
            Assert.IsNotNull(_spawnArea);
            Assert.IsNotNull(_playerTransform);
        }
        
        private void OnEnable() => EndlessModeManager.OnWaveStarted += StartWave;
        private void OnDisable() => EndlessModeManager.OnWaveStarted -= StartWave;

        private void StartWave(int waveNumber)
        {
            int count = waveNumber * 5; 
            StartCoroutine(SpawnRoutine(count));
        }

        private IEnumerator SpawnRoutine(int count)
        {
            _isSpawning = true;
            for (int i = 0; i < count; i++)
            {
                SpawnEnemyAtRandomPoint();
                yield return new WaitForSeconds(_timeBetweenSpawns);
            }
            _isSpawning = false;
        }

        private void SpawnEnemyAtRandomPoint()
        {
            if (_enemyPrefabs.Length == 0 || _spawnArea == null) return;

            Bounds bounds = _spawnArea.bounds;
            Vector2 spawnPos = Vector2.zero;
            bool validPosition = false;
            int attempts = 0;

            // The loop now checks if the point is INSIDE the collider shape
            while (!validPosition && attempts < 30) // Increased attempts for precision
            {
                float x = Random.Range(bounds.min.x, bounds.max.x);
                float y = Random.Range(bounds.min.y, bounds.max.y);
                spawnPos = new Vector2(x, y);

                // Check 1: Is it actually inside the painted tiles/shape?
                bool isInside = _spawnArea.OverlapPoint(spawnPos);

                // Check 2: Is it far from the player?
                bool farFromPlayer = true;
                if (_playerTransform != null)
                {
                    float distance = Vector2.Distance(spawnPos, (Vector2)_playerTransform.position);
                    farFromPlayer = (distance >= _minSpawnDistance);
                }

                if (isInside && farFromPlayer)
                {
                    validPosition = true;
                }

                attempts++;
            }

            // Fallback: If we can't find a spot after 30 tries, spawn at the spawner's center
            // to prevent the game from breaking.
            if (!validPosition) spawnPos = transform.position;

            GameObject prefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
            GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

            var enemyDetection = enemy.GetComponent<EnemyDetection>();
            if (enemyDetection != null)
            {
                enemyDetection.IncreaseDetectionRange(1000f);
            }

            _activeEnemies.Add(enemy);
        }

        private void Update()
        {
            if (_isSpawning) return;

            _activeEnemies.RemoveAll(item => item == null || !item.activeInHierarchy);

            if (_activeEnemies.Count == 0)
            {
                _manager.CompleteWave();
            }
        }
    }
}