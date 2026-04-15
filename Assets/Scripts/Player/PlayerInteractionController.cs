using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    [SerializeField]
    private PlayerStats _stats;

    [SerializeField]
    private Rigidbody2D _rigidBody;

    [SerializeField]
    private Collider2D _detectionCollider;

    private readonly List<Collider2D> _overlapResults = new();

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        GetClosestInteractable()?.Interact();
    }

    private IInteractable GetClosestInteractable()
    {
        ContactFilter2D filter = new()
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = Physics2D.GetLayerCollisionMask(_detectionCollider.gameObject.layer),
        };

        Physics2D.OverlapCollider(_detectionCollider, filter, _overlapResults);

        IInteractable closest = null;
        float closestDistanceSq = float.MaxValue;
        Vector2 playerPosition = transform.position;

        for (int i = 0; i < _overlapResults.Count; i++)
        {
            if (!_overlapResults[i].CompareTag(GlobalConstants.INTERACTABLE_TAG))
            {
                continue;
            }

            if (!_overlapResults[i].TryGetComponent(out IInteractable interactable))
            {
                continue;
            }

            if (interactable is MonoBehaviour behaviour)
            {
                float distanceSq = (
                    (Vector2)behaviour.transform.position - playerPosition
                ).sqrMagnitude;

                if (distanceSq < closestDistanceSq)
                {
                    closestDistanceSq = distanceSq;
                    closest = interactable;
                }
            }
        }

        return closest;
    }
}
