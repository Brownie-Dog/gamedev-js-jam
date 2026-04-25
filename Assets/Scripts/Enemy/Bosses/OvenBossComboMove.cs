using System;
using UnityEngine;

namespace Enemy.Bosses
{
    public class OvenBossComboMove : MonoBehaviour, IOvenBossMove
    {
        public bool IsComplete { get; private set; }
        public bool IsLaunched { get; private set; }
        public event Action OnMoveComplete;

        public void Execute(Transform boss, Transform player)
        {
            IsComplete = true;
            OnMoveComplete?.Invoke();
        }
    }
}
