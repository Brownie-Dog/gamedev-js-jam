using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SimpleEnemyMovementAnimation : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController _baseController;
    [SerializeField] private AnimationClip _idleClip;
    [SerializeField] private AnimationClip _walkClip;
    [SerializeField] private float _moveThreshold = 0.1f;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Animator _animator;

    private static readonly int IsMovingHash = Animator.StringToHash("isMoving");
    private static readonly int MoveXHash = Animator.StringToHash("moveX");

    private void Awake()
    {
        Assert.IsNotNull(_baseController);
        Assert.IsNotNull(_rb);
        Assert.IsNotNull(_animator);
        Assert.IsNotNull(_spriteRenderer);

        var overrideController = new AnimatorOverrideController { runtimeAnimatorController = _baseController };
        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        overrideController.GetOverrides(overrides);

        overrides.Sort((a, b) =>
            string.Compare(a.Key ? a.Key.name : "", b.Key ? b.Key.name : "", StringComparison.Ordinal)
        );

        for (int i = 0; i < overrides.Count; i++)
        {
            if (i == 0 && _idleClip != null && overrides[i].Key != null)
            {
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, _idleClip);
            }
            else if (i == 1 && _walkClip != null && overrides[i].Key != null)
            {
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, _walkClip);
            }
        }

        overrideController.ApplyOverrides(overrides);
        _animator.runtimeAnimatorController = overrideController;
    }

    private void Update()
    {
        Vector2 velocity = _rb.linearVelocity;
        bool isMoving = velocity.magnitude > _moveThreshold;

        _animator.SetBool(IsMovingHash, isMoving);
        _animator.SetFloat(MoveXHash, velocity.x);

        if (Mathf.Abs(velocity.x) > 0.01f)
        {
            _spriteRenderer.flipX = velocity.x < 0;
        }
    }
}