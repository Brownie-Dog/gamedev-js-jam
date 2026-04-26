using System;
using UnityEngine;

namespace Enemy.Bosses.FanMan
{
    public interface IFanManMove
    {
        void Execute(Transform boss, Transform player);
        bool IsComplete { get; }
        bool IsActive { get; }
        event Action OnMoveComplete;
    }
}
