using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SimpleEnemyMovementAnimation : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController _baseController;
    [SerializeField] private AnimationClip _baseIdleClip;
    [SerializeField] private AnimationClip _baseWalkClip;
    [SerializeField] private AnimationClip _idleClip;
    [SerializeField] private AnimationClip _walkClip;
    [SerializeField] private float _moveThreshold = 0.1f;
    [SerializeField] private bool _flipSprite = true;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Animator _animator;

    private static readonly int IsMovingHash = Animator.StringToHash("isMoving");
    private static readonly int MoveXHash = Animator.StringToHash("moveX");

    private void Awake()
    {
        Assert.IsNotNull(_baseController);
        Assert.IsNotNull(_baseIdleClip);
        Assert.IsNotNull(_baseWalkClip);
        Assert.IsNotNull(_rb);
        Assert.IsNotNull(_animator);
        Assert.IsNotNull(_spriteRenderer);

        var overrideController = new AnimatorOverrideController { runtimeAnimatorController = _baseController };
        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        overrideController.GetOverrides(overrides);

        for (int i = 0; i < overrides.Count; i++)
        {
            var original = overrides[i].Key;
            if (original == _baseIdleClip && _idleClip != null)
            {
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(original, _idleClip);
            }
            else if (original == _baseWalkClip && _walkClip != null)
            {
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(original, _walkClip);
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

        if (_flipSprite && Mathf.Abs(velocity.x) > 0.01f)
        {
            _spriteRenderer.flipX = velocity.x < 0;
        }
    }
}