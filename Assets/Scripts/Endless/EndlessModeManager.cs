using UnityEngine;
using System;

namespace EndlessMode
{
    public enum EndlessState { Intermission, WaveActive, GameOver }

    public class EndlessModeManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _startingWave = 1;

        public static event Action<int> OnWaveStarted;
        public static event Action OnIntermissionStarted;

        private int _currentWave;
        private EndlessState _currentState;

        public int CurrentWave => _currentWave;

        private void Start()
        {
            _currentWave = _startingWave;
            EnterIntermission();
        }

        // Called by the SafeZoneTrigger
        public void StartNextWave()
        {
            if (_currentState != EndlessState.Intermission) return;

            _currentState = EndlessState.WaveActive;
            OnWaveStarted?.Invoke(_currentWave);
            Debug.Log($"Endless Mode: Starting Wave {_currentWave}");
        }

        // To be called by your Enemy Spawner when all enemies die
        public void CompleteWave()
        {
            if (_currentState != EndlessState.WaveActive) return;

            _currentWave++;
            EnterIntermission();
        }

        private void EnterIntermission()
        {
            _currentState = EndlessState.Intermission;
            OnIntermissionStarted?.Invoke();
            Debug.Log("Endless Mode: Intermission Started. Visit the Shop!");
        }
        
        public void EndRun()
        {
            _currentState = EndlessState.GameOver;
            // Handle Game Over UI here
        }
    }
}