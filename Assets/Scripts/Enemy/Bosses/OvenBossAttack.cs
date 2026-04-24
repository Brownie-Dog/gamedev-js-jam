using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Enemy.Bosses
{
    public class OvenBossAttack : EnemyAttack
    {
        [SerializeField] private List<MonoBehaviour> _moveBehaviours;
        [SerializeField] private Transform _bossTransform;
        [SerializeField] private Transform _playerTransform;

        private List<IOvenBossMove> _moves;
        private IOvenBossMove _currentMove;

        private void Awake()
        {
            Assert.IsNotNull(_bossTransform);
            Assert.IsNotNull(_playerTransform);

            _moves = new List<IOvenBossMove>(_moveBehaviours.Count);
            foreach (var behaviour in _moveBehaviours)
            {
                Assert.IsNotNull(behaviour);
                var move = behaviour as IOvenBossMove;
                Assert.IsTrue(move != null, $"{behaviour.name} does not implement IOvenBossMove");
                _moves.Add(move);
            }
        }

        public bool IsMoveComplete => _currentMove == null || _currentMove.IsComplete;

        protected override void Attack()
        {
            _currentMove = _moves[Random.Range(0, _moves.Count)];
            Debug.Log($"[OvenBossAttack]Executing move: {_currentMove.GetType().Name}");
            _currentMove.Execute(_bossTransform, _playerTransform);
        }
    }
}