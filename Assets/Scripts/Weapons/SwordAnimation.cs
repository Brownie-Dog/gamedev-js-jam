using System.Collections;
using UnityEngine;

namespace Weapons
{
    public class SwordAnimation : MonoBehaviour, IWeaponAnimation
    {
        [SerializeField] private float _attackDuration = 0.15f;

        public IEnumerator PlayAnimation()
        {
            var elapsed = 0f;
            var startRotation = transform.localRotation.eulerAngles.z;

            while (elapsed < _attackDuration)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / _attackDuration;
                var angle = Mathf.Lerp(startRotation, startRotation - 360f, t);
                transform.localRotation = Quaternion.Euler(0, 0, angle);

                yield return null;
            }

            transform.localRotation = Quaternion.Euler(0, 0, startRotation);
        }
    }
}