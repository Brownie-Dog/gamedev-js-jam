using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Enemy.Bosses
{
    public class OvenBossComboMove : MonoBehaviour, IOvenBossMove
    {
        [SerializeField] private OvenBossArmSpawner _armSpawner;
        [SerializeField] private OvenBossPunchMove _punchMove;
        [SerializeField] private OvenBossGrabMove _grabMove;
        [SerializeField] private OvenBossSwordMove _swordMove;
        [SerializeField] private EnemyMovement _bossMovement;

        [SerializeField] private OvenBossMoveType _firstMove;
        [SerializeField] private OvenBossMoveType _secondMove;

        private Coroutine _comboRoutine;
        private bool _isLaunched;
        private bool _firstAttackRetracting;

        public bool IsComplete { get; private set; }
        public bool IsLaunched => _isLaunched;
        public bool IsAttacking => _isLaunched;
        public event Action OnMoveComplete;

        public bool IsArmComplete(OvenBossArm arm) => IsComplete;
        public bool IsArmLaunched(OvenBossArm arm) => IsLaunched;
        public bool IsArmAttacking(OvenBossArm arm) => IsAttacking;

        private void Awake()
        {
            Assert.IsNotNull(_armSpawner);
            Assert.IsNotNull(_punchMove);
            Assert.IsNotNull(_grabMove);
            Assert.IsNotNull(_swordMove);
            Assert.IsNotNull(_bossMovement);
        }

        public void Execute(Transform boss, Transform player)
        {
            IsComplete = false;
            _isLaunched = false;
            _firstAttackRetracting = false;

            if (_comboRoutine != null)
            {
                StopCoroutine(_comboRoutine);
            }

            _comboRoutine = StartCoroutine(ComboRoutine(player));
        }

        private IEnumerator ComboRoutine(Transform player)
        {
            bool firstIsLeft = Random.value < 0.5f;
            OvenBossArm firstArm = firstIsLeft ? _armSpawner.LeftArm : _armSpawner.RightArm;
            OvenBossArm secondArm = firstIsLeft ? _armSpawner.RightArm : _armSpawner.LeftArm;

            IOvenBossMove first = GetMove(_firstMove);
            IOvenBossMove second = GetMove(_secondMove);

            SetAttackGate(first, null);
            SetAttackGate(second, () => !IsFirstAttacking(first));

            if (_firstMove == OvenBossMoveType.Grab || _secondMove == OvenBossMoveType.Grab)
            {
                _grabMove.ResetGrabState();
            }

            _isLaunched = true;
            _bossMovement.PauseMovement();

            var firstCore = ExecuteCore(first, firstArm, player);
            var secondCore = ExecuteCore(second, secondArm, player);

            Coroutine firstC = StartCoroutine(firstCore);
            Coroutine secondC = StartCoroutine(secondCore);
            yield return firstC;
            yield return secondC;

            _isLaunched = false;
            IsComplete = true;
            _bossMovement.ResumeMovement();
            OnMoveComplete?.Invoke();
            _comboRoutine = null;
        }

        private bool IsFirstAttacking(IOvenBossMove move)
        {
            if (move is OvenBossPunchMove punch) return punch.IsAttacking;
            if (move is OvenBossSwordMove sword) return sword.IsAttacking;
            if (move is OvenBossGrabMove grab) return grab.IsAttacking;
            return false;
        }

        private IOvenBossMove GetMove(OvenBossMoveType type)
        {
            return type switch
            {
                OvenBossMoveType.Punch => _punchMove,
                OvenBossMoveType.Sword => _swordMove,
                OvenBossMoveType.Grab => _grabMove,
                _ => _punchMove
            };
        }

        private void SetAttackGate(IOvenBossMove move, Func<bool> gate)
        {
            if (move is OvenBossPunchMove punch) punch.SetAttackGate(gate);
            else if (move is OvenBossSwordMove sword) sword.SetAttackGate(gate);
            else if (move is OvenBossGrabMove grab) grab.SetAttackGate(gate);
        }

        private IEnumerator ExecuteCore(IOvenBossMove move, OvenBossArm arm, Transform player)
        {
            if (move is OvenBossPunchMove punch)
                return punch.ExecuteCore(arm, player);
            else if (move is OvenBossSwordMove sword)
                return sword.ExecuteCore(arm, player);
            else if (move is OvenBossGrabMove grab)
                return grab.ExecuteCore(arm, player);
            return null;
        }
    }
}
