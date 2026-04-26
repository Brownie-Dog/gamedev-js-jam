using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Enemy.Bosses
{
    public class OvenBossDoubleGrabMove : MonoBehaviour, IOvenBossMove
    {
        [SerializeField] private OvenBossArmSpawner _armSpawner;
        [SerializeField] private OvenBossGrabMove _grabMove;
        [SerializeField] private EnemyMovement _bossMovement;

        private Coroutine _doubleGrabRoutine;

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
            Assert.IsNotNull(_grabMove);
            Assert.IsNotNull(_bossMovement);
        }

        public void Execute(Transform boss, Transform player)
        {
            IsComplete = false;
            IsLaunched = false;

            if (_doubleGrabRoutine != null)
            {
                StopCoroutine(_doubleGrabRoutine);
            }

            _doubleGrabRoutine = StartCoroutine(DoubleGrabRoutine(player));
        }

        private IEnumerator DoubleGrabRoutine(Transform player)
        {
            _grabMove.ResetGrabState();

            IsLaunched = true;
            _bossMovement.PauseMovement();

            var leftCore = _grabMove.ExecuteCore(_armSpawner.LeftArm, player);
            var rightCore = _grabMove.ExecuteCore(_armSpawner.RightArm, player);

            Coroutine left = StartCoroutine(leftCore);
            Coroutine right = StartCoroutine(rightCore);
            yield return left;
            yield return right;

            IsLaunched = false;
            IsComplete = true;
            _bossMovement.ResumeMovement();
            OnMoveComplete?.Invoke();
            _doubleGrabRoutine = null;
        }
    }
}
