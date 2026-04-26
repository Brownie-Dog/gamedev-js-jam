using UnityEngine;

namespace Enemy.Bosses
{
    public class FanPushParticleController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particles;
        [SerializeField] private BoxCollider2D _zoneCollider;
        [SerializeField] private float _startupEmissionRate = 20f;
        [SerializeField] private float _pushEmissionRate = 80f;

        [Header("Velocity Over Lifetime")]
        [Tooltip("Y-velocity during startup (positive = up toward fan)")]
        [SerializeField] private float _startupVelocityY = 2f;
        [Tooltip("Y-velocity during push (negative = down away from fan)")]
        [SerializeField] private float _pushVelocityY = -8f;

        private bool _isConfigured;

        private void Awake()
        {
            FindParticles();

            if (_particles != null)
            {
                var main = _particles.main;
                main.playOnAwake = false;
                _particles.Stop(withChildren: true, stopBehavior: ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            else
            {
                Debug.LogError($"[FanPushParticles] No ParticleSystem found on '{gameObject.name}' or its children.", this);
            }

        }

        private void OnDisable()
        {
            if (_particles != null)
            {
                _particles.Stop(withChildren: true, stopBehavior: ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        private void FindParticles()
        {
            if (_particles != null) return;

            _particles = GetComponent<ParticleSystem>();
            if (_particles == null)
                _particles = GetComponentInChildren<ParticleSystem>(true);
        }

        public void Configure(BoxCollider2D zoneCollider)
        {
            _zoneCollider = zoneCollider;
            FindParticles();
            _isConfigured = true;
        }

        public void PlayStartup()
        {
            PlayInternal(_startupEmissionRate, _startupVelocityY);
        }

        public void PlayPush()
        {
            PlayInternal(_pushEmissionRate, _pushVelocityY);
        }

        private void PlayInternal(float rate, float velocityY)
        {
            FindParticles();

            if (_particles == null)
            {
                Debug.LogWarning($"[FanPushParticles] PlayInternal: _particles is NULL", this);
                return;
            }

            // Ensure the GameObject holding THIS script is active
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }

            // Ensure the GameObject holding the ParticleSystem is active
            GameObject psGO = _particles.gameObject;
            if (!psGO.activeInHierarchy)
            {
                psGO.SetActive(true);
            }

            if (!_isConfigured && _zoneCollider != null)
                Configure(_zoneCollider);

            // Apply emission rate
            var emission = _particles.emission;
            if (!emission.enabled)
            {
                emission.enabled = true;
            }
            emission.rateOverTime = rate;

            // Apply velocity over lifetime (up for startup, down for push)
            var vel = _particles.velocityOverLifetime;
            if (!vel.enabled)
            {
                vel.enabled = true;
                vel.space = ParticleSystemSimulationSpace.Local;
            }
            vel.y = new ParticleSystem.MinMaxCurve(velocityY);

            _particles.Play(withChildren: true);
        }

        public void Stop()
        {
            if (_particles == null) return;
            _particles.Stop(withChildren: true, stopBehavior: ParticleSystemStopBehavior.StopEmitting);
        }

        public void Clear()
        {
            if (_particles == null) return;
            _particles.Clear(withChildren: true);
        }
    }
}
