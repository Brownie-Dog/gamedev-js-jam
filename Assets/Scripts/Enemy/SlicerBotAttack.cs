using System.Collections;
using UnityEngine;

public class SlicerBotAttack : MeleeEnemyAttack
{
    [SerializeField] private Transform _weaponTransform;
    [SerializeField] private float _windUpTime = 0.4f;
    [SerializeField] private float _strikeTime = 0.15f;
    [SerializeField] private float _startAngle = 30f;
    [SerializeField] private float _peakAngle = 120f;
    [SerializeField] private float _strikeAngle = -10f;

    protected override void Attack()
    {
        StartCoroutine(TelegraphedAttackRoutine());
    }

    private IEnumerator TelegraphedAttackRoutine()
    {
        var timer = 0f;

        while (timer < _windUpTime)
        {
            timer += Time.deltaTime;
            var t = timer / _windUpTime;
            var currentZ = Mathf.Lerp(_startAngle, _peakAngle, t);
            _weaponTransform.localRotation = Quaternion.Euler(0, 0, currentZ);
            yield return null;
        }

        timer = 0f;
        var damageDealt = false;

        while (timer < _strikeTime)
        {
            timer += Time.deltaTime;
            var t = timer / _strikeTime;
            var currentZ = Mathf.Lerp(_peakAngle, _strikeAngle, t);
            _weaponTransform.localRotation = Quaternion.Euler(0, 0, currentZ);

            if (t >= 0.5f && !damageDealt)
            {
                damageDealt = true;
            }

            yield return null;
        }

        _weaponTransform.localRotation = Quaternion.Euler(0, 0, _startAngle);
    }
}