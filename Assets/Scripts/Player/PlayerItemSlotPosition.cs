using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public class PlayerItemSlotPosition : MonoBehaviour
{
    [Header("Formation Data Asset")] [SerializeField]
    private ItemFormationData _formationData;

    [Header("Slot References")] [SerializeField]
    private Transform _frontLeftSlot;

    [SerializeField] private Transform _frontRightSlot;
    [SerializeField] private Transform _backLeftSlot;
    [SerializeField] private Transform _backRightSlot;
    [SerializeField] private Transform _tailSlot;
    [SerializeField] private Transform _middleSlot;

    [Header("Sorting Group References")] 
    [SerializeField] private SortingGroup _frontLeftSorting;
    [SerializeField] private SortingGroup _frontRightSorting;
    [SerializeField] private SortingGroup _backLeftSorting;
    [SerializeField] private SortingGroup _backRightSorting;
    [SerializeField] private SortingGroup _tailSorting;
    [SerializeField] private SortingGroup _middleSorting;

    private void Awake()
    {
        Assert.IsNotNull(_frontLeftSorting);
        Assert.IsNotNull(_frontRightSorting);
        Assert.IsNotNull(_backLeftSorting);
        Assert.IsNotNull(_backRightSorting);
        Assert.IsNotNull(_tailSorting);
        Assert.IsNotNull(_middleSorting);
        Assert.IsNotNull(_formationData);
        Assert.IsNotNull(_frontLeftSlot);
        Assert.IsNotNull(_frontRightSlot);
        Assert.IsNotNull(_backLeftSlot);
        Assert.IsNotNull(_backRightSlot);
        Assert.IsNotNull(_tailSlot);
        Assert.IsNotNull(_middleSlot);
    }

    public void UpdateSlotLayout(Vector2 direction)
    {
        if (direction == Vector2.up)
            ApplyPositions(_formationData.UpPositions);
        else if (direction == Vector2.down)
            ApplyPositions(_formationData.DownPositions);
        else if (direction == Vector2.right)
            ApplyPositions(_formationData.RightPositions);
        else
            ApplyPositions(_formationData.LeftPositions);
    }

    private void ApplyPositions(in ItemFormationData.SlotGroup group)
    {
        UpdatePhysicalLayout(group);
        UpdateVisualDepth(group);
    }

    private void UpdatePhysicalLayout(in ItemFormationData.SlotGroup group)
    {
        SetTransform(_frontLeftSlot, group.FrontLeft, group.FrontLeftRot);
        SetTransform(_frontRightSlot, group.FrontRight, group.FrontRightRot);
        SetTransform(_backLeftSlot, group.BackLeft, group.BackLeftRot);
        SetTransform(_backRightSlot, group.BackRight, group.BackRightRot);
        SetTransform(_tailSlot, group.Tail, group.TailRot);
        SetTransform(_middleSlot, group.Middle, group.MiddleRot);
    }

    private void UpdateVisualDepth(in ItemFormationData.SlotGroup group)
    {
        SetOrder(_frontLeftSorting, group.FrontLeftLayer);
        SetOrder(_frontRightSorting, group.FrontRightLayer);
        SetOrder(_backLeftSorting, group.BackLeftLayer);
        SetOrder(_backRightSorting, group.BackRightLayer);
        SetOrder(_tailSorting, group.TailLayer);
        SetOrder(_middleSorting, group.MiddleLayer);
    }

    private static void SetTransform(Transform slot, Vector2 pos, float rot)
    {
        slot.localPosition = pos;
        slot.localRotation = Quaternion.Euler(0, 0, rot);
    }

    private static void SetOrder(SortingGroup group, int order)
    {
        group.sortingOrder = order;
    }
}