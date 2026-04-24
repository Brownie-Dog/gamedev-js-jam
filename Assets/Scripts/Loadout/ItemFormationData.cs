using UnityEngine;

[CreateAssetMenu(fileName = "RobotFormationData", menuName = "Robot/Formation Data")]
public class ItemFormationData : ScriptableObject
{
    [System.Serializable]
    public class SlotGroup
    {
        [Header("Positions")] 
        public Vector2 frontLeft;
        public Vector2 frontRight;
        public Vector2 backLeft;
        public Vector2 backRight;
        public Vector2 tail;
        public Vector2 middle;

        [Header("Rotations (Z-axis)")] [Range(-360, 360)]
        public float frontLeftRot;

        [Range(-360, 360)] public float frontRightRot;
        [Range(-360, 360)] public float backLeftRot;
        [Range(-360, 360)] public float backRightRot;
        [Range(-360, 360)] public float tailRot;
        [Range(-360, 360)] public float middleRot;

        [Header("Sorting Layers")] public int frontLeftLayer;
        public int frontRightLayer;
        public int backLeftLayer;
        public int backRightLayer;
        public int tailLayer;
        public int middleLayer;

        public SlotGroup(Vector2 fl, Vector2 fr, Vector2 bl, Vector2 br, Vector2 t, Vector2 m, int layer = 10)
        {
            frontLeft = fl;
            frontRight = fr;
            backLeft = bl;
            backRight = br;
            tail = t;
            middle = m;
            frontLeftLayer = frontRightLayer = backLeftLayer = backRightLayer = tailLayer = middleLayer = layer;
            frontLeftRot = frontRightRot = backLeftRot = backRightRot = tailRot = middleRot = 0;
        }
    }

    [Header("Directional Formations")] public SlotGroup upPositions;
    public SlotGroup downPositions;
    public SlotGroup leftPositions;
    public SlotGroup rightPositions;

    private void Reset()
    {
        InitializeHardcodedValues();
    }

    [ContextMenu("Re-Initialize Values")]
    public void InitializeHardcodedValues()
    {
        upPositions = new SlotGroup(
            new Vector2(-0.5f, 0.6f), new Vector2(0.5f, 0.6f),
            new Vector2(-0.5f, -0.3f), new Vector2(0.5f, -0.3f),
            new Vector2(0f, -0.6f), new Vector2(0f, 0.2f)
        );

        downPositions = new SlotGroup(
            new Vector2(-0.6f, 0.06f), new Vector2(0.6f, 0.06f),
            new Vector2(-0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0f, 1.3f), new Vector2(0f, 0.7f)
        );

        rightPositions = new SlotGroup(
            new Vector2(0.5f, 0.7f), new Vector2(0.5f, 0.1f),
            new Vector2(-0.65f, 0.75f), new Vector2(-0.65f, 0.15f),
            new Vector2(-1f, 0.5f), new Vector2(-0.1f, 0.5f)
        );

        leftPositions = new SlotGroup(
            new Vector2(-0.5f, 0.7f), new Vector2(-0.5f, 0.1f),
            new Vector2(0.65f, 0.75f), new Vector2(0.65f, 0.15f),
            new Vector2(1f, 0.5f), new Vector2(0.1f, 0.5f)
        );
    }
}