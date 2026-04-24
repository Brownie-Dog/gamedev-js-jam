using UnityEngine;

[CreateAssetMenu(fileName = "RobotFormationData", menuName = "Robot/Formation Data")]
public class ItemFormationData : ScriptableObject
{
    [System.Serializable]
    public class SlotGroup
    {
        [Header("Positions")] 
        public Vector2 FrontLeft;
        public Vector2 FrontRight;
        public Vector2 BackLeft;
        public Vector2 BackRight;
        public Vector2 Tail;
        public Vector2 Middle;

        [Header("Rotations (Z-axis)")] 
        [Range(-360, 360)] public float FrontLeftRot;
        [Range(-360, 360)] public float FrontRightRot;
        [Range(-360, 360)] public float BackLeftRot;
        [Range(-360, 360)] public float BackRightRot;
        [Range(-360, 360)] public float TailRot;
        [Range(-360, 360)] public float MiddleRot;

        [Header("Sorting Layers")] 
        public int FrontLeftLayer;
        public int FrontRightLayer;
        public int BackLeftLayer;
        public int BackRightLayer;
        public int TailLayer;
        public int MiddleLayer;

        public SlotGroup(Vector2 fl, Vector2 fr, Vector2 bl, Vector2 br, Vector2 t, Vector2 m, int layer = 10)
        {
            FrontLeft = fl;
            FrontRight = fr;
            BackLeft = bl;
            BackRight = br;
            Tail = t;
            Middle = m;
            FrontLeftLayer = FrontRightLayer = BackLeftLayer = BackRightLayer = TailLayer = MiddleLayer = layer;
            FrontLeftRot = FrontRightRot = BackLeftRot = BackRightRot = TailRot = MiddleRot = 0;
        }
    }

    [Header("Directional Formations")] public SlotGroup UpPositions;
    public SlotGroup DownPositions;
    public SlotGroup LeftPositions;
    public SlotGroup RightPositions;

    private void Reset()
    {
        InitializeHardcodedValues();
    }

    [ContextMenu("Re-Initialize Values")]
    public void InitializeHardcodedValues()
    {
        UpPositions = new SlotGroup(
            new Vector2(-0.5f, 0.6f), new Vector2(0.5f, 0.6f),
            new Vector2(-0.5f, -0.3f), new Vector2(0.5f, -0.3f),
            new Vector2(0f, -0.6f), new Vector2(0f, 0.2f)
        );

        DownPositions = new SlotGroup(
            new Vector2(0.6f, -0.06f), new Vector2(0.6f, 0.06f),
            new Vector2(-0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0f, 1.3f), new Vector2(0f, 0.7f)
        );

        RightPositions = new SlotGroup(
            new Vector2(0.5f, 0.7f), new Vector2(0.5f, 0.1f),
            new Vector2(-0.65f, 0.75f), new Vector2(-0.65f, 0.15f),
            new Vector2(-1f, 0.5f), new Vector2(-0.1f, 0.5f)
        );

        LeftPositions = new SlotGroup(
            new Vector2(-0.5f, 0.7f), new Vector2(-0.5f, 0.1f),
            new Vector2(0.65f, 0.75f), new Vector2(0.65f, 0.15f),
            new Vector2(1f, 0.5f), new Vector2(0.1f, 0.5f)
        );
    }
}