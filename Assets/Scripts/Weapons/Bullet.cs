using System;
using Player;
using UnityEngine;
using UnityEngine.Pool;

namespace Weapons
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(DamageDealer))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _speed = 20f;
        [SerializeField] private float _lifetime = 3f;

        private Rigidbody2D _rb;
        private DamageDealer _damageDealer;
        private IObjectPool<Bullet> _pool;
        private float _lifetimeTimer;
        private int _currentPenetration;
        private bool _returnedToPool;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _damageDealer = GetComponent<DamageDealer>();
        }

        private void OnEnable()
        {
            _damageDealer.OnHit += HandleHit;
        }

        private void OnDisable()
        {
            _damageDealer.OnHit -= HandleHit;
        }

        public void Activate(Vector2 position, Vector2 direction, DamageInfo damageInfo, int penetrationCount)
        {
            _returnedToPool = false;
            transform.position = position;
            transform.up = direction;
            _currentPenetration = penetrationCount;
            _damageDealer.Activate(damageInfo);
            _rb.linearVelocity = direction.normalized * _speed;
            _lifetimeTimer = _lifetime;
        }

        private void HandleHit()
        {
            _currentPenetration--;

            if (_currentPenetration <= 0)
            {
                ReturnToPool();
            }
        }

        public void Deactivate()
        {
            _damageDealer.Deactivate();
            _rb.linearVelocity = Vector2.zero;
        }

        private void Update()
        {
            _lifetimeTimer -= Time.deltaTime;
            if (_lifetimeTimer <= 0f)
            {
                ReturnToPool();
            }
        }

        private void ReturnToPool()
        {
            if (_returnedToPool) return;
            _returnedToPool = true;
            _pool.Release(this);
        }

        public void SetPool(IObjectPool<Bullet> pool)
        {
            _pool = pool;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                ReturnToPool();
            }
        }
    }
}