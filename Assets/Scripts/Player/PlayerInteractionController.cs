using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    [SerializeField]
    private PlayerStatsSo _statsSo;

    [SerializeField]
    private Rigidbody2D _rigidBody;

    [SerializeField]
    private Collider2D _detectionCollider;

    private readonly List<Collider2D> _overlapResults = new();
    private IInteractable _currentInteractable;

    public event Action<bool> OnInteractableInRange;

    private void Awake()
    {
        Assert.IsNotNull(_statsSo);
        Assert.IsNotNull(_rigidBody);
    }

    private void Update()
    {
        var closest = GetClosestInteractable();
        if (closest != _currentInteractable)
        {
            _currentInteractable = closest;
            OnInteractableInRange?.Invoke(_currentInteractable != null);
        }
    }

    public void InputEvent_OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        _currentInteractable?.Interact();
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
