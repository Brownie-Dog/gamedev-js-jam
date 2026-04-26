using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Enemy.Bosses
{
    public class FlameProjectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 10f;

        private Vector2 _startPosition;
        private Vector2 _target;
        private float _flightDuration;
        private float _flightTimer;
        private bool _targetSet;

        private MonoBehaviourPool<FlameProjectile> _pool;
        private Action<Vector2> _spawnFireTile;

        public void SetPool(MonoBehaviourPool<FlameProjectile> pool)
        {
            _pool = pool;
        }

        public void SetSpawnFireTileAction(Action<Vector2> action)
        {
            _spawnFireTile = action;
        }

        public void SetTarget(Vector2 target)
        {
            _target = target;
            _startPosition = transform.position;
            float distance = Vector2.Distance(_startPosition, _target);
            _flightDuration = distance / _speed;
            _flightTimer = 0f;
            _targetSet = true;
        }

        private void Update()
        {
            if (!_targetSet)
            {
                return;
            }

            _flightTimer += Time.deltaTime;
            float t = Mathf.Clamp01(_flightTimer / _flightDuration);
            float easedT = t * t;

            transform.position = Vector2.Lerp(_startPosition, _target, easedT);

            if (t >= 1f)
            {
                Impact();
            }
        }

        private void Impact()
        {
            _spawnFireTile?.Invoke(_target);
            _targetSet = false;
            _pool?.Pool.Release(this);
        }
    }
}