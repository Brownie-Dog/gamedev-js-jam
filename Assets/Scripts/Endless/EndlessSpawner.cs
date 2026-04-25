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
        [SerializeField] private Transform[] _spawnPoints;

        [Header("Settings")]
        [SerializeField] private float _timeBetweenSpawns = 1.5f;

        private List<GameObject> _activeEnemies = new List<GameObject>();
        private bool _isSpawning = false;

        private void OnEnable()
        {
            EndlessModeManager.OnWaveStarted += StartWave;
        }

        private void OnDisable()
        {
            EndlessModeManager.OnWaveStarted -= StartWave;
        }

        private void StartWave(int waveNumber)
        {
            // Simple logic: Wave 1 = 5 enemies, Wave 2 = 10, etc.
            int count = waveNumber * 5; 
            StartCoroutine(SpawnRoutine(count));
        }

        private IEnumerator SpawnRoutine(int count)
        {
            _isSpawning = true;
            
            for (int i = 0; i < count; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(_timeBetweenSpawns);
            }

            _isSpawning = false;
        }

        private void SpawnEnemy()
        {
            if (_enemyPrefabs.Length == 0 || _spawnPoints.Length == 0) return;

            Transform sp = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
            GameObject prefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
            
            GameObject enemy = Instantiate(prefab, sp.position, Quaternion.identity);
            _activeEnemies.Add(enemy);
        }

        private void Update()
        {
            if (_isSpawning) return;

            // Clean up dead enemies from the list
            _activeEnemies.RemoveAll(item => item == null);

            // Check if wave is cleared
            if (_activeEnemies.Count == 0)
            {
                _manager.CompleteWave();
            }
        }
    }
}