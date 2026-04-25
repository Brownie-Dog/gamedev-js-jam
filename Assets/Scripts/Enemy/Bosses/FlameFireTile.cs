using UnityEngine;
using UnityEngine.Assertions;

namespace Enemy.Bosses
{
    public class FlameFireTile : MonoBehaviour
    {
        [SerializeField] private float _lifetime = 5f;
        [SerializeField] private float _fadeOutDuration = 0.5f;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private MonoBehaviourPool<FlameFireTile> _pool;

        public void SetPool(MonoBehaviourPool<FlameFireTile> pool)
        {
            _pool = pool;
        }

        public void ResetLifetime()
        {
            Color color = _spriteRenderer.color;
            color.a = 1f;
            _spriteRenderer.color = color;

            StopAllCoroutines();
            StartCoroutine(LifetimeRoutine());
        }

        private void Awake()
        {
            Assert.IsNotNull(_spriteRenderer);
        }

        private System.Collections.IEnumerator LifetimeRoutine()
        {
            yield return new WaitForSeconds(_lifetime - _fadeOutDuration);

            float timer = 0f;
            Color color = _spriteRenderer.color;
            float startAlpha = color.a;

            while (timer < _fadeOutDuration)
            {
                timer += Time.deltaTime;
                float t = timer / _fadeOutDuration;
                color.a = Mathf.Lerp(startAlpha, 0f, t);
                _spriteRenderer.color = color;
                yield return null;
            }

            color.a = 0f;
            _spriteRenderer.color = color;

            _pool?.Pool.Release(this);
        }
    }
}
