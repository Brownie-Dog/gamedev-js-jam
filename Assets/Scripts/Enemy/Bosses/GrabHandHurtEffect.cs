using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Enemy.Bosses
{
    public class GrabHandHurtEffect : MonoBehaviour
    {
        [SerializeField] private float _flashDuration = 0.15f;
        [SerializeField] private Color _flashColor = Color.red;

        private OvenBossArm _arm;
        private GrabHand _grabHand;
        private Coroutine _hurtCoroutine;

        private void Awake()
        {
            Assert.IsNotNull(_arm = GetComponent<OvenBossArm>());
        }

        private void OnEnable()
        {
            _arm.OnHandSwapped += HandleHandSwapped;
            SubscribeToGrabHand();
        }

        private void OnDisable()
        {
            _arm.OnHandSwapped -= HandleHandSwapped;
            UnsubscribeFromGrabHand();
        }

        private void HandleHandSwapped(object sender, EventArgs e)
        {
            if (_hurtCoroutine != null)
            {
                StopCoroutine(_hurtCoroutine);
                _hurtCoroutine = null;
            }

            UnsubscribeFromGrabHand();
            SubscribeToGrabHand();
        }

        private void SubscribeToGrabHand()
        {
            _grabHand = GetComponentInChildren<GrabHand>();
            if (_grabHand != null)
            {
                _grabHand.OnHit += HandleHit;
            }
        }

        private void UnsubscribeFromGrabHand()
        {
            if (_grabHand != null)
            {
                _grabHand.OnHit -= HandleHit;
                _grabHand = null;
            }
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
            SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            float elapsed = 0f;

            while (elapsed < _flashDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _flashDuration;

                foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                {
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.color = Color.Lerp(_flashColor, Color.white, t);
                    }
                }

                yield return null;
            }

            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.white;
                }
            }

            _hurtCoroutine = null;
        }
    }
}
