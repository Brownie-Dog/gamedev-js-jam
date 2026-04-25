using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EndlessMode
{
    public class EndlessSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EndlessModeManager _manager;
        [SerializeField] private GameObject[] _enemyPrefabs;
        
        [Header("Spawn Zone")]
        [SerializeField] private Collider2D _spawnArea; // Drag your 'Arena' collider here

        [Header("Settings")]
        [SerializeField] private float _timeBetweenSpawns = 1.0f;

        [Header("Safe Spawn Settings")]
        [SerializeField] private Transform _playerTransform; // Drag Player here
        [SerializeField] private float _minSpawnDistance = 5f;
        
        
        private List<GameObject> _activeEnemies = new List<GameObject>();
        private bool _isSpawning = false;

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

            // Try to find a spot away from the player
            while (!validPosition && attempts < 10)
            {
                float x = Random.Range(bounds.min.x, bounds.max.x);
                float y = Random.Range(bounds.min.y, bounds.max.y);
                spawnPos = new Vector2(x, y);

                // If no player reference, any spot is valid
                if (_playerTransform == null) 
                {
                    validPosition = true;
                }
                else
                {
                    // Check distance between random spot and player
                    float distance = Vector2.Distance(spawnPos, (Vector2)_playerTransform.position);
                    if (distance >= _minSpawnDistance)
                    {
                        validPosition = true;
                    }
                }
                attempts++;
            }

            GameObject prefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
            GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
            _activeEnemies.Add(enemy);
        }

        private void Update()
        {
            if (_isSpawning) return;

            _activeEnemies.RemoveAll(item => item == null);

            if (_activeEnemies.Count == 0)
            {
                _manager.CompleteWave();
            }
        }
    }
}