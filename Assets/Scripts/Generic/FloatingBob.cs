using UnityEngine;

namespace Generic
{
    /// <summary>
    /// Smoothly bobs a child visual transform up and down using a sine wave.
    /// Attach to the parent (e.g. shop pedestal or item drop root) and wire the visual child.
    /// </summary>
    public class FloatingBob : MonoBehaviour
    {
        [SerializeField] private Transform _visualTransform;
        [Tooltip("How far up/down the visual moves from its base position.")]
        [SerializeField] private float _amplitude = 0.15f;
        [Tooltip("Seconds for one full up-down bob cycle.")]
        [SerializeField] private float _cycleDuration = 2f;
        [Tooltip("Optional phase offset in seconds to desync nearby items.")]
        [SerializeField] private float _phaseOffset;

        private float _baseY;

        private void Awake()
        {
            if (_visualTransform == null)
            {
                _visualTransform = transform;
            }

            _baseY = _visualTransform.localPosition.y;
        }

        private void Update()
        {
            float frequency = 1f / Mathf.Max(_cycleDuration, 0.001f);
            float t = (Time.time + _phaseOffset) * frequency * Mathf.PI * 2f;
            float offset = Mathf.Sin(t) * _amplitude;
            Vector3 pos = _visualTransform.localPosition;
            pos.y = _baseY + offset;
            _visualTransform.localPosition = pos;
        }
    }
}
