using UnityEngine;
using System;

namespace Enemy.Bosses
{
    public interface IOvenBossMove
    {
        void Execute(Transform boss, Transform player);
        bool IsComplete { get; }
        event Action OnMoveComplete;
    }
}