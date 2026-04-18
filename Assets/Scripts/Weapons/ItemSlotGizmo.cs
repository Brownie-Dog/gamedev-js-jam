using UnityEngine;

namespace Weapons
{
    public class ItemSlotGizmo : MonoBehaviour
    {
        [SerializeField] private float _arrowLength = 1f;
        [SerializeField] private float _arrowHeadAngle = 20f;
        [SerializeField] private float _rotationOffset = 0f;
        [SerializeField] private Color _arrowColor = Color.yellow;

        private void OnDrawGizmos()
        {
            Gizmos.color = _arrowColor;
            Vector3 origin = transform.position;
            Vector3 direction = Quaternion.Euler(0, 0, _rotationOffset) * transform.right;

            Vector3 end = origin + direction * _arrowLength;
            Vector3 leftDir = Quaternion.Euler(0, 0, _arrowHeadAngle) * direction;
            Vector3 rightDir = Quaternion.Euler(0, 0, -_arrowHeadAngle) * direction;

            Gizmos.DrawLine(origin, end);
            Gizmos.DrawLine(end, end + leftDir * 0.2f * _arrowLength);
            Gizmos.DrawLine(end, end + rightDir * 0.2f * _arrowLength);
        }
    }
}