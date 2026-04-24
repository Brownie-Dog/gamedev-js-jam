using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Enemy.Bosses
{
    public class OvenBossPunchMove : MonoBehaviour, IOvenBossMove
    {
        [SerializeField] private OvenBossArmSpawner _armSpawner;
        [SerializeField] private GameObject _closedClawHandPrefab;
        [SerializeField] private EnemyStats _stats;
        [SerializeField] private EnemyMovement _bossMovement;
        [SerializeField] private float _aimDurationMin = 2f;
        [SerializeField] private float _aimDurationMax = 5f;
        [SerializeField] private float _hitPauseDuration = 0.2f;

        private Player.DamageDealer _damageDealer;
        private Coroutine _punchRoutine;

        public bool IsComplete { get; private set; }
        public event Action OnMoveComplete;

        private void Awake()
        {
            Assert.IsNotNull(_armSpawner);
            Assert.IsNotNull(_closedClawHandPrefab);
            Assert.IsNotNull(_stats);
            Assert.IsNotNull(_bossMovement);
        }

        public void Execute(Transform boss, Transform player)
        {
            IsComplete = false;
            _bossMovement.PauseMovement();

            if (_punchRoutine != null)
            {
                StopCoroutine(_punchRoutine);
            }

            _punchRoutine = StartCoroutine(PunchRoutine(boss, player));
        }

        private IEnumerator PunchRoutine(Transform boss, Transform player)
        {
            OvenBossArm arm = Random.value < 0.5f ? _armSpawner.LeftArm : _armSpawner.RightArm;
            var armController = arm.GetComponent<OvenBossArmController>();
            armController.SetPlayer(player);

            arm.SwapHand(_closedClawHandPrefab);
            _damageDealer = arm.GetHandComponent<Player.DamageDealer>();
            Assert.IsNotNull(_damageDealer, "ClosedClawHand prefab must have a DamageDealer component");

            var damageInfo = new DamageInfo(_stats.Damage, Vector2.one * _stats.KnockbackForce);

            yield return armController.AimPhase(Random.Range(_aimDurationMin, _aimDurationMax));

            _damageDealer.Activate(damageInfo);

            yield return armController.LaunchTowardPlayer();
            yield return new WaitForSeconds(_hitPauseDuration);

            _damageDealer.Deactivate();
            _damageDealer = null;

            yield return armController.RetractToDefault();

            arm.SwapToDefaultHand();
            IsComplete = true;
            _bossMovement.ResumeMovement();
            OnMoveComplete?.Invoke();
            _punchRoutine = null;
        }
    }
}