using UnityEngine;

namespace Weapons
{
    public class HookPoint : MonoBehaviour
    {
        [field: SerializeField]
        public SlotType SlotType { get; private set; } = SlotType.General;
    }
}