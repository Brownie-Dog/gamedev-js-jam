using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

namespace Enemy.Bosses
{
    public class FanManRailgunBeamSegment : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private BoxCollider2D _collider;
        [SerializeField] private ContinuousDamageZone _damageZone;
        [SerializeField] private float _beamWidth = 0.2f;

        private IObjectPool<FanManRailgunBeamSegment> _pool;
        private Coroutine _lingerRoutine;
        private System.Action _onReleased;

        private void Awake()
        {
            Assert.IsNotNull(_lineRenderer);
            Assert.IsNotNull(_collider);
            Assert.IsNotNull(_damageZone);
            _lineRenderer.useWorldSpace = true;
        }

        public void SetPool(IObjectPool<FanManRailgunBeamSegment> pool)
        {
            _pool = pool;
        }

        public void Setup(Vector2 start, Vector2 end, float lingerDuration, int damage, float knockbackForce, System.Action onReleased = null)
        {
            _onReleased = onReleased;

            if (_lingerRoutine != null)
            {
                StopCoroutine(_lingerRoutine);
                _lingerRoutine = null;
            }

            // LineRenderer
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, start);
            _lineRenderer.SetPosition(1, end);
            _lineRenderer.startWidth = _beamWidth;
            _lineRenderer.endWidth = _beamWidth;
            _lineRenderer.startColor = Color.white;
            _lineRenderer.endColor = Color.white;

            // Collider aligned to segment
            Vector2 center = (start + end) / 2f;
            float length = Vector2.Distance(start, end);
            Vector2 dir = (end - start).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            transform.position = center;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            _collider.size = new Vector2(length, _beamWidth);
            _collider.offset = Vector2.zero;

            // Damage
            _damageZone.Configure(damage, dir * knockbackForce);
            _damageZone.Activate();

            _lingerRoutine = StartCoroutine(LingerRoutine(lingerDuration));
        }

        private IEnumerator LingerRoutine(float duration)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = 1f - (timer / duration);
                Color c = new Color(1f, 1f, 1f, Mathf.Clamp01(t));
                _lineRenderer.startColor = c;
                _lineRenderer.endColor = new Color(1f, 1f, 1f, Mathf.Clamp01(t * 0.7f));
                yield return null;
            }

            _damageZone.Deactivate();
            _lineRenderer.startColor = Color.clear;
            _lineRenderer.endColor = Color.clear;

            if (_onReleased != null)
            {
                _onReleased.Invoke();
            }

            if (_pool != null)
            {
                _pool.Release(this);
            }

            _lingerRoutine = null;
        }

        private void OnDisable()
        {
            if (_lingerRoutine != null)
            {
                StopCoroutine(_lingerRoutine);
                _lingerRoutine = null;
            }

            _damageZone.Deactivate();
        }
    }
}
