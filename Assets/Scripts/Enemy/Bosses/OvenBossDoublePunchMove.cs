using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Enemy.Bosses
{
    public class OvenBossDoublePunchMove : MonoBehaviour, IOvenBossMove
    {
        [SerializeField] private OvenBossArmSpawner _armSpawner;
        [SerializeField] private OvenBossPunchMove _punchMove;
        [SerializeField] private EnemyMovement _bossMovement;

        private Coroutine _doublePunchRoutine;

        public bool IsComplete { get; private set; }
        public bool IsLaunched { get; private set; }
        public bool IsAttacking => IsLaunched;
        public event Action OnMoveComplete;

        public bool IsArmComplete(OvenBossArm arm) => IsComplete;
        public bool IsArmLaunched(OvenBossArm arm) => IsLaunched;
        public bool IsArmAttacking(OvenBossArm arm) => IsAttacking;

        private void Awake()
        {
            Assert.IsNotNull(_armSpawner);
            Assert.IsNotNull(_punchMove);
            Assert.IsNotNull(_bossMovement);
        }

        public void Execute(Transform boss, Transform player)
        {
            IsComplete = false;
            IsLaunched = false;

            if (_doublePunchRoutine != null)
            {
                StopCoroutine(_doublePunchRoutine);
            }

            _doublePunchRoutine = StartCoroutine(DoublePunchRoutine(player));
        }

        private IEnumerator DoublePunchRoutine(Transform player)
        {
            IsLaunched = true;
            _bossMovement.PauseMovement();

            var leftCore = _punchMove.ExecuteCore(_armSpawner.LeftArm, player);
            var rightCore = _punchMove.ExecuteCore(_armSpawner.RightArm, player);

            Coroutine left = StartCoroutine(leftCore);
            Coroutine right = StartCoroutine(rightCore);
            yield return left;
            yield return right;

            IsLaunched = false;
            IsComplete = true;
            _bossMovement.ResumeMovement();
            OnMoveComplete?.Invoke();
            _doublePunchRoutine = null;
        }
    }
}
