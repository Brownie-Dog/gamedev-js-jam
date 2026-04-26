using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyHurtEffect : MonoBehaviour
{
    [SerializeField] private EnemyHealth _enemyHealth;
    [SerializeField] private float _flashDuration = 0.15f;
    [SerializeField] private Color _flashColor = Color.red;

    private SpriteRenderer[] _spriteRenderers;
    private Coroutine _hurtCoroutine;

    private void Awake()
    {
        Assert.IsNotNull(_enemyHealth);
        RefreshSpriteRenderers();
    }

    private void OnEnable()
    {
        _enemyHealth.OnHealthChanged += HandleHit;
    }

    private void OnDisable()
    {
        _enemyHealth.OnHealthChanged -= HandleHit;
    }

    public void RefreshSpriteRenderers()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private void HandleHit(object sender, EventArgs e)
    {
        if (_hurtCoroutine != null)
        {
            StopCoroutine(_hurtCoroutine);
        }

        _hurtCoroutine = StartCoroutine(HurtRoutine());
    }

    private IEnumerator HurtRoutine()
    {
        var elapsed = 0f;

        while (elapsed < _flashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _flashDuration;

            foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
            {
                spriteRenderer.color = Color.Lerp(_flashColor, Color.white, t);
            }

            yield return null;
        }

        foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
        {
            spriteRenderer.color = Color.white;
        }

        _hurtCoroutine = null;
    }
}