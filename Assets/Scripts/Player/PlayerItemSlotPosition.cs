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

    public void UpdateSlotLayout(Vector2 direction, bool isLeft)
    {
        transform.localScale = new Vector3(isLeft ? -1f : 1f, 1f, 1f);
        if (direction == Vector2.up)
            ApplyPositions(_formationData.upPositions);
        else if (direction == Vector2.down)
            ApplyPositions(_formationData.downPositions);
        else
            ApplyPositions(_formationData.sidePositions);
    }

    private void ApplyPositions(ItemFormationData.SlotGroup group)
    {
        UpdatePhysicalLayout(group);
        UpdateVisualDepth(group);
    }

    private void UpdatePhysicalLayout(ItemFormationData.SlotGroup group)
    {
        SetTransform(_frontLeftSlot, group.frontLeft, group.frontLeftRot);
        SetTransform(_frontRightSlot, group.frontRight, group.frontRightRot);
        SetTransform(_backLeftSlot, group.backLeft, group.backLeftRot);
        SetTransform(_backRightSlot, group.backRight, group.backRightRot);
        SetTransform(_tailSlot, group.tail, group.tailRot);
        SetTransform(_middleSlot, group.middle, group.middleRot);
    }

    private void UpdateVisualDepth(ItemFormationData.SlotGroup group)
    {
        SetSlotSortingGroup(_frontLeftSlot, group.leftLegsLayer);
        SetSlotSortingGroup(_backLeftSlot, group.leftLegsLayer);
        SetSlotSortingGroup(_frontRightSlot, group.rightLegsLayer);
        SetSlotSortingGroup(_backRightSlot, group.rightLegsLayer);
        SetSlotSortingGroup(_tailSlot, group.tailLayer);
        SetSlotSortingGroup(_middleSlot, group.middleLayer);
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