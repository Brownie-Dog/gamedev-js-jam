using System;
using UnityEngine;

namespace Weapons
{
    public class AimDirectionArgs : EventArgs
    {
        public readonly Vector2 Direction;

        public AimDirectionArgs(Vector2 direction)
        {
            Direction = direction;
        }
    }
}