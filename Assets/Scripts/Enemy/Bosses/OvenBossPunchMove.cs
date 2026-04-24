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
        [SerializeField] private float _aimDurationMin = 2f;
        [SerializeField] private float _aimDurationMax = 5f;
        [SerializeField] private int _extraSegments = 3;
        [SerializeField] private int _damage = 2;
        [SerializeField] private float _knockbackForce = 10f;
        [SerializeField] private float _hitPauseDuration = 0.2f;

        private Player.DamageDealer _damageDealer;
        private Coroutine _punchRoutine;

        public bool IsComplete { get; private set; }

        private void Awake()
        {
            Assert.IsNotNull(_armSpawner);
            Assert.IsNotNull(_closedClawHandPrefab);
        }

        public void Execute(Transform boss, Transform player)
        {
            IsComplete = false;

            Debug.Log("[OvenBossPunchMove] Execute called");

            if (_punchRoutine != null)
            {
                StopCoroutine(_punchRoutine);
            }

            _punchRoutine = StartCoroutine(PunchRoutine(boss, player));
        }

        private IEnumerator PunchRoutine(Transform boss, Transform player)
        {
            OvenBossArm arm = Random.value < 0.5f ? _armSpawner.LeftArm : _armSpawner.RightArm;

            arm.SwapHand(_closedClawHandPrefab);
            _damageDealer = arm.GetHandComponent<Player.DamageDealer>();
            Assert.IsNotNull(_damageDealer, "ClosedClawHand prefab must have a DamageDealer component");

            var damageInfo = new DamageInfo(_damage, Vector2.one * _knockbackForce);
            _damageDealer.Activate(damageInfo);

            yield return AimPhase(arm, player);
            yield return LaunchPhase(arm, player);
            yield return new WaitForSeconds(_hitPauseDuration);
            yield return RetractPhase(arm);

            _damageDealer.Deactivate();
            _damageDealer = null;

            arm.SwapToDefaultHand();
            IsComplete = true;
            _punchRoutine = null;
        }

        private IEnumerator AimPhase(OvenBossArm arm, Transform player)
        {
            float aimDuration = Random.Range(_aimDurationMin, _aimDurationMax);
            float timer = 0f;

            while (timer < aimDuration)
            {
                timer += Time.deltaTime;
                Vector2 direction = (Vector2)(player.position - arm.HandPosition.position).normalized;
                arm.SetDirection(direction);
                yield return null;
            }
        }

        private IEnumerator LaunchPhase(OvenBossArm arm, Transform player)
        {
            float distanceToPlayer = Vector2.Distance(arm.HandPosition.position, player.position);
            int targetSegments = arm.CalculateTargetSegments(distanceToPlayer) + _extraSegments;

            arm.ExtendTo(targetSegments);

            while (arm.IsExtending)
            {
                yield return null;
            }
        }

        private IEnumerator RetractPhase(OvenBossArm arm)
        {
            arm.RetractToDefault();

            while (arm.IsRetracting)
            {
                yield return null;
            }
        }
    }
}