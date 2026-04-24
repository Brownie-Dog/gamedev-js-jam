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

    private void Awake()
    {
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

    private void ApplyPositions(ItemFormationData.SlotGroup group)
    {
        UpdatePhysicalLayout(group);
        UpdateVisualDepth(group);
    }

    private void UpdatePhysicalLayout(ItemFormationData.SlotGroup group)
    {
        SetTransform(_frontLeftSlot, group.FrontLeft, group.FrontLeftRot);
        SetTransform(_frontRightSlot, group.FrontRight, group.FrontRightRot);
        SetTransform(_backLeftSlot, group.BackLeft, group.BackLeftRot);
        SetTransform(_backRightSlot, group.BackRight, group.BackRightRot);
        SetTransform(_tailSlot, group.Tail, group.TailRot);
        SetTransform(_middleSlot, group.Middle, group.MiddleRot);
    }

    private void UpdateVisualDepth(ItemFormationData.SlotGroup group)
    {
        SetSlotSortingGroup(_frontLeftSlot, group.FrontLeftLayer);
        SetSlotSortingGroup(_frontRightSlot, group.FrontRightLayer);
        SetSlotSortingGroup(_backLeftSlot, group.BackLeftLayer);
        SetSlotSortingGroup(_backRightSlot, group.BackRightLayer);
        SetSlotSortingGroup(_tailSlot, group.TailLayer);
        SetSlotSortingGroup(_middleSlot, group.MiddleLayer);
    }

    private static void SetTransform(Transform slot, Vector2 pos, float rot)
    {
        slot.localPosition = pos;
        slot.localRotation = Quaternion.Euler(0, 0, rot);
    }

    private static void SetSlotSortingGroup(Transform slot, int order)
    {
        var sortingGroup = slot.GetComponent<SortingGroup>();
        if (sortingGroup)
        {
            sortingGroup.sortingOrder = order;
        }
    }
}