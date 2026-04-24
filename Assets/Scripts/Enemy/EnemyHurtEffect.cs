using System;
using System.Collections;
using UnityEngine;

public class EnemyHurtEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private EnemyHealth _enemyHealth;
    [SerializeField] private float _flashDuration = 0.15f;
    [SerializeField] private Color _flashColor = Color.red;

    private Coroutine _hurtCoroutine;

    private void OnEnable()
    {
        _enemyHealth.OnHealthChanged += HandleHit;
    }

    private void OnDisable()
    {
        _enemyHealth.OnHealthChanged -= HandleHit;
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
        float elapsed = 0f;

        while (elapsed < _flashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _flashDuration;
            _spriteRenderer.color = Color.Lerp(_flashColor, Color.white, t);
            yield return null;
        }

        _spriteRenderer.color = Color.white;
        _hurtCoroutine = null;
    }
}