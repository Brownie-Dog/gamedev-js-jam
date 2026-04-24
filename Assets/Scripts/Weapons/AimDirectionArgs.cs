using System;
using UnityEngine;

namespace Weapons
{
    public class AimDirectionArgs : EventArgs
    {
        public readonly Vector3 TargetPosition;

        public AimDirectionArgs(Vector3 targetPosition)
        {
            TargetPosition = targetPosition;
        }
    }
}