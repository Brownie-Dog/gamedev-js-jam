using UnityEngine;

public class ColliderVisualiser : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private Color _color = Color.green;

    private void OnDrawGizmosSelected()
    {
        if (_collider == null)
        {
            _collider = GetComponent<Collider2D>();
        }

        if (_collider == null)
        {
            return;
        }

        Gizmos.color = _color;

        if (_collider is CircleCollider2D circle)
        {
            Gizmos.DrawWireSphere(transform.TransformPoint(circle.offset), circle.radius * transform.lossyScale.x);
        }
        else if (_collider is BoxCollider2D box)
        {
            var center = transform.TransformPoint(box.offset);
            var size = Vector2.Scale(box.size, transform.lossyScale);
            Gizmos.DrawWireCube(center, size);
        }
    }
}