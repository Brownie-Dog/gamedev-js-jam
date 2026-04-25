using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Enemy.Bosses
{
    public class OvenBossSwordMove : MonoBehaviour, IOvenBossMove
    {
        [SerializeField] private OvenBossArmSpawner _armSpawner;
        [SerializeField] private GameObject _swordHandPrefab;
        [SerializeField] private EnemyStats _stats;
        [SerializeField] private EnemyMovement _enemyMovement;
        [SerializeField] private float _telegraphDurationMin = 2f;
        [SerializeField] private float _telegraphDurationMax = 5f;
        [SerializeField] private float _swingSpeed = 180f;
        [SerializeField] private float _sweepAngle = 135f;
        [SerializeField] private int _swordDamage = 3;
        [SerializeField] private float _swordKnockbackForce = 30f;
        [SerializeField] private float _swordKnockbackDuration = 0.5f;
        [SerializeField] private float _hitPauseDuration = 0.2f;

        private Player.DamageDealer _damageDealer;
        private Coroutine _swordRoutine;

        public bool IsComplete { get; private set; }
        public event Action OnMoveComplete;

        private static readonly Vector2 West = Vector2.left;
        private static readonly Vector2 East = Vector2.right;

        private void Awake()
        {
            Assert.IsNotNull(_armSpawner);
            Assert.IsNotNull(_swordHandPrefab);
            Assert.IsNotNull(_stats);
            Assert.IsNotNull(_enemyMovement);
        }

        public void Execute(Transform boss, Transform player)
        {
            IsComplete = false;
            _enemyMovement.PauseMovement();

            if (_swordRoutine != null)
            {
                StopCoroutine(_swordRoutine);
            }

            _swordRoutine = StartCoroutine(SwordRoutine(boss, player));
        }

        private IEnumerator SwordRoutine(Transform boss, Transform player)
        {
            bool isLeftArm = Random.value < 0.5f;
            OvenBossArm arm = isLeftArm ? _armSpawner.LeftArm : _armSpawner.RightArm;
            var armController = arm.GetComponent<OvenBossArmController>();
            armController.SetPlayer(player);

            arm.SwapHand(_swordHandPrefab);

            _damageDealer = arm.GetHandComponent<Player.DamageDealer>();
            Assert.IsNotNull(_damageDealer, "SwordHand prefab must have a DamageDealer component");

            Vector2 startDirection = isLeftArm ? West : East;
            float sweepAngle = isLeftArm ? _sweepAngle : -_sweepAngle;
            Vector2 knockback = isLeftArm ? East : West;

            var damageInfo = new DamageInfo(_swordDamage, knockback * _swordKnockbackForce, _swordKnockbackDuration);

            yield return armController.TelegraphPhase(startDirection, Random.Range(_telegraphDurationMin, _telegraphDurationMax));

            _damageDealer.Activate(damageInfo);

            yield return armController.SweepPhase(startDirection, sweepAngle, _swingSpeed);

            _damageDealer.Deactivate();
            _damageDealer = null;

            yield return new WaitForSeconds(_hitPauseDuration);

            arm.SwapToDefaultHand();
            IsComplete = true;
            _enemyMovement.ResumeMovement();
            OnMoveComplete?.Invoke();
            _swordRoutine = null;
        }
    }
}
