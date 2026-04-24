using System.Collections;
using UnityEngine;

namespace Weapons
{
    public class SawbladeAnimation : MonoBehaviour, IWeaponAnimation
    {
        [SerializeField] private float _rotationsPerSecond = 3f;

        private float _currentRotation;

        private void Update()
        {
            float speed = _rotationsPerSecond * 360f;
            _currentRotation -= speed * Time.deltaTime;
            
            transform.localRotation = Quaternion.Euler(0, 0, _currentRotation);
        }

        public IEnumerator PlayAnimation()
        {
            yield return null;
        }
    }
}