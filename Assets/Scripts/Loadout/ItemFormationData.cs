using UnityEngine;

[CreateAssetMenu(fileName = "RobotFormationData", menuName = "Robot/Formation Data")]
public class ItemFormationData : ScriptableObject
{
    [System.Serializable]
    public struct SlotGroup
    {
        [Header("Positions")] public Vector2 FrontLeft;
        public Vector2 FrontRight;
        public Vector2 BackLeft;
        public Vector2 BackRight;
        public Vector2 Tail;
        public Vector2 Middle;

        [Header("Rotations (Z-axis)")] [Range(-360, 360)]
        public float FrontLeftRot;

        [Range(-360, 360)] public float FrontRightRot;
        [Range(-360, 360)] public float BackLeftRot;
        [Range(-360, 360)] public float BackRightRot;
        [Range(-360, 360)] public float TailRot;
        [Range(-360, 360)] public float MiddleRot;

        [Header("Sorting Layers")] public int FrontLeftLayer;
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

    [Header("Directional Formations")]
    [field: SerializeField]
    public SlotGroup UpPositions { get; private set; }

    [field: SerializeField] public SlotGroup DownPositions { get; private set; }
    [field: SerializeField] public SlotGroup LeftPositions { get; private set; }
    [field: SerializeField] public SlotGroup RightPositions { get; private set; }

    private void Reset()
    {
        InitializeHardcodedValues();
    }

    [ContextMenu("Re-Initialize Values")]
    public void InitializeHardcodedValues()
    {
        UpPositions = new SlotGroup
        {
            FrontLeft = new Vector2(-0.5f, 0.6f), FrontRight = new Vector2(0.5f, 0.6f),
            BackLeft = new Vector2(-0.5f, -0.3f), BackRight = new Vector2(0.5f, -0.3f),
            Tail = new Vector2(0f, -0.6f), Middle = new Vector2(0f, 0.2f),
            
            FrontLeftRot = -315f, FrontRightRot = -315f,
            BackLeftRot = 135f, BackRightRot = -45f,
            TailRot = -135f, MiddleRot = 45f,
            
            FrontLeftLayer = 11, FrontRightLayer = 11,
            BackLeftLayer = 15, BackRightLayer = 15,
            TailLayer = 13, MiddleLayer = 13
        };
        
        DownPositions = new SlotGroup
        {
            FrontLeft = new Vector2(0.6f, 0.06f), FrontRight = new Vector2(-0.6f, 0.06f),
            BackLeft = new Vector2(0.5f, 1f), BackRight = new Vector2(-0.5f, 1f),
            Tail = new Vector2(0f, 1.3f), Middle = new Vector2(0f, 0.7f),
            
            FrontLeftRot = -135f, FrontRightRot = -135f,
            BackLeftRot = -45f, BackRightRot = 135f,
            TailRot = 45f, MiddleRot = -135f,
            
            FrontLeftLayer = 13, FrontRightLayer = 13,
            BackLeftLayer = 13, BackRightLayer = 13,
            TailLayer = 13, MiddleLayer = 13
        };
        
        LeftPositions = new SlotGroup
        {
            FrontLeft = new Vector2(-0.5f, 0.1f), FrontRight = new Vector2(-0.5f, 0.7f),
            BackLeft = new Vector2(0.65f, 0.15f), BackRight = new Vector2(0.65f, 0.75f),
            Tail = new Vector2(1f, 0.5f), Middle = new Vector2(0.1f, 0.5f),
            
            FrontLeftRot = -225f, FrontRightRot = -225f,
            BackLeftRot = -135f, BackRightRot = 45f,
            TailRot = -45f, MiddleRot = 135f,
            
            FrontLeftLayer = 15, FrontRightLayer = 11,
            BackLeftLayer = 15, BackRightLayer = 11,
            TailLayer = 13, MiddleLayer = 13
        };
        
        RightPositions = new SlotGroup
        {
            FrontLeft = new Vector2(0.5f, 0.7f), FrontRight = new Vector2(0.5f, 0.1f),
            BackLeft = new Vector2(-0.65f, 0.75f), BackRight = new Vector2(-0.65f, 0.15f),
            Tail = new Vector2(-1f, 0.5f), Middle = new Vector2(-0.1f, 0.5f),
            
            FrontLeftRot = -45f, FrontRightRot = -45f,
            BackLeftRot = 45f, BackRightRot = -135f,
            TailRot = 135f, MiddleRot = -45f,
            
            FrontLeftLayer = 11, FrontRightLayer = 15,
            BackLeftLayer = 11, BackRightLayer = 15,
            TailLayer = 13, MiddleLayer = 13
        };
    }
}