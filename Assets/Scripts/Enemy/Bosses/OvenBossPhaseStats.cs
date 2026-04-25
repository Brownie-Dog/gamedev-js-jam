using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss/OvenBossPhaseStats")]
public class OvenBossPhaseStats : ScriptableObject
{
    public PhaseStats Phase1 = new()
    {
        ArmCooldown = 4f,
        AimDurationMin = 2f,
        AimDurationMax = 5f,
        ArmSpeedMultiplier = 1f,
        MovementSpeedMultiplier = 1f,
        PunchWeight = 0.4f,
        GrabWeight = 0.3f,
        SwordWeight = 0.3f,
        SpecialMoveWeight = 0f,
        DoublePunchWeight = 0f,
        DoubleGrabWeight = 0f,
        ComboWeight = 0f
    };

    public PhaseStats Phase2 = new()
    {
        ArmCooldown = 2.5f,
        AimDurationMin = 1.5f,
        AimDurationMax = 3.5f,
        ArmSpeedMultiplier = 1.5f,
        MovementSpeedMultiplier = 1.2f,
        PunchWeight = 0.35f,
        GrabWeight = 0.3f,
        SwordWeight = 0.35f,
        SpecialMoveWeight = 0f,
        DoublePunchWeight = 0f,
        DoubleGrabWeight = 0f,
        ComboWeight = 0f
    };

    public PhaseStats Phase3 = new()
    {
        ArmCooldown = 1.5f,
        AimDurationMin = 1f,
        AimDurationMax = 2.5f,
        ArmSpeedMultiplier = 2f,
        MovementSpeedMultiplier = 1.5f,
        PunchWeight = 0.3f,
        GrabWeight = 0.25f,
        SwordWeight = 0.3f,
        SpecialMoveWeight = 0.25f,
        DoublePunchWeight = 0.35f,
        DoubleGrabWeight = 0.3f,
        ComboWeight = 0.35f
    };

    [Serializable]
    public class PhaseStats
    {
        public float ArmCooldown;
        public float AimDurationMin;
        public float AimDurationMax;
        public float ArmSpeedMultiplier = 1f;
        public float MovementSpeedMultiplier = 1f;
        [Range(0f, 1f)] public float PunchWeight;
        [Range(0f, 1f)] public float GrabWeight;
        [Range(0f, 1f)] public float SwordWeight;
        [Range(0f, 1f)] public float SpecialMoveWeight;
        [Range(0f, 1f)] public float DoublePunchWeight;
        [Range(0f, 1f)] public float DoubleGrabWeight;
        [Range(0f, 1f)] public float ComboWeight;
    }
}
