using System;
using UnityEngine;

namespace Enemy.Bosses
{
    [CreateAssetMenu(menuName = "Boss/FanManPhaseStats")]
    public class FanManPhaseStats : ScriptableObject
    {
        public PhaseStats Phase1 = new()
        {
            MoveCooldown = 4f,
            AimDurationMin = 2f,
            AimDurationMax = 5f,
            MovementSpeedMultiplier = 1f,
            FanPushDuration = 2f,
            FanPushSlowMultiplier = 0.5f,
            NormalGunShotMin = 2,
            NormalGunShotMax = 3,
            NormalGunShotInterval = 0.2f,
            RailgunTelegraphDuration = 2f,
            RailgunLingerDuration = 1.5f,
            RailgunMaxRange = 60f,
            RailgunMultiShotMin = 1,
            RailgunMultiShotMax = 1,
            RailgunTelegraphInterval = 0.5f,
            RailgunAimVarianceDeg = 0f,
            RailgunAimVarianceThreshold = 1f,
            FanPushWeight = 0.5f,
            NormalGunWeight = 0.5f,
            RailgunWeight = 0f,
            DualHandGunWeight = 0f,
            ConcurrentMoveWeight = 0f
        };

        public PhaseStats Phase2 = new()
        {
            MoveCooldown = 2.5f,
            AimDurationMin = 1.5f,
            AimDurationMax = 3.5f,
            MovementSpeedMultiplier = 1.2f,
            FanPushDuration = 2f,
            FanPushSlowMultiplier = 0.5f,
            NormalGunShotMin = 3,
            NormalGunShotMax = 4,
            NormalGunShotInterval = 0.2f,
            RailgunTelegraphDuration = 1.5f,
            RailgunLingerDuration = 1.5f,
            RailgunMaxRange = 60f,
            RailgunMultiShotMin = 1,
            RailgunMultiShotMax = 1,
            RailgunTelegraphInterval = 0.5f,
            RailgunAimVarianceDeg = 0f,
            RailgunAimVarianceThreshold = 1f,
            FanPushWeight = 0.45f,
            NormalGunWeight = 0.4f,
            RailgunWeight = 0.15f,
            DualHandGunWeight = 0.5f,
            ConcurrentMoveWeight = 0f
        };

        public PhaseStats Phase3 = new()
        {
            MoveCooldown = 1.5f,
            AimDurationMin = 1f,
            AimDurationMax = 2.5f,
            MovementSpeedMultiplier = 1.5f,
            FanPushDuration = 2f,
            FanPushSlowMultiplier = 0.5f,
            NormalGunShotMin = 4,
            NormalGunShotMax = 5,
            NormalGunShotInterval = 0.15f,
            RailgunTelegraphDuration = 1f,
            RailgunLingerDuration = 2f,
            RailgunMaxRange = 60f,
            RailgunMultiShotMin = 2,
            RailgunMultiShotMax = 3,
            RailgunTelegraphInterval = 0.5f,
            RailgunAimVarianceDeg = 10f,
            RailgunAimVarianceThreshold = 1f,
            FanPushWeight = 0.35f,
            NormalGunWeight = 0.3f,
            RailgunWeight = 0.35f,
            DualHandGunWeight = 0.75f,
            ConcurrentMoveWeight = 0.35f
        };

        [Serializable]
        public class PhaseStats
        {
            [Header("Cooldowns & Speed")]
            public float MoveCooldown = 4f;
            public float AimDurationMin = 2f;
            public float AimDurationMax = 5f;
            public float MovementSpeedMultiplier = 1f;

            [Header("Fan Push")]
            public float FanPushDuration = 2f;
            public float FanPushSlowMultiplier = 0.5f;

            [Header("Gun")]
            public int NormalGunShotMin = 2;
            public int NormalGunShotMax = 3;
            public float NormalGunShotInterval = 0.2f;

            [Header("Railgun")]
            public float RailgunTelegraphDuration = 2f;
            public float RailgunLingerDuration = 1.5f;
            public float RailgunMaxRange = 30f;
            public int RailgunMultiShotMin = 1;
            public int RailgunMultiShotMax = 1;
            public float RailgunTelegraphInterval = 0.5f;
            public float RailgunAimVarianceDeg = 0f;
            public float RailgunAimVarianceThreshold = 1f;

            [Header("Move Weights")]
            [Range(0f, 1f)] public float FanPushWeight = 0.5f;
            [Range(0f, 1f)] public float NormalGunWeight = 0.5f;
            [Range(0f, 1f)] public float RailgunWeight = 0f;

            [Header("Advanced")]
            [Range(0f, 1f)] public float DualHandGunWeight = 0f;
            [Range(0f, 1f)] public float ConcurrentMoveWeight = 0f;
        }
    }
}
