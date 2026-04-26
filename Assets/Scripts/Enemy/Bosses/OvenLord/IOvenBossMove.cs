using UnityEngine;
using System;

namespace Enemy.Bosses
{
    public interface IOvenBossMove
    {
        void Execute(Transform boss, Transform player);
        bool IsComplete { get; }
        bool IsLaunched { get; }
        bool IsAttacking { get; }
        event Action OnMoveComplete;

        bool IsArmComplete(OvenBossArm arm);
        bool IsArmLaunched(OvenBossArm arm);
        bool IsArmAttacking(OvenBossArm arm);
    }
}
