using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Weapons
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Sword : SecondaryWeapon
    {
        [SerializeField]
        private ItemDrops.WeaponItemData _weaponData;

        [SerializeField]
        private float _attackDuration = 0.15f;

        private SpriteRenderer _renderer;
        private bool _isAttacking;

        protected override void Awake()
        {
            base.Awake();

            _renderer = GetComponent<SpriteRenderer>();
            Assert.IsNotNull(_renderer);

            Assert.IsNotNull(_weaponData);
        }

        private void Start()
        {
            _renderer.sprite = _weaponData.Icon;
        }

        protected override void OnSecondaryAttack(object obj, EventArgs args)
        {
            Attack();
        }

        public void Attack()
        {
            if (_isAttacking)
            {
                return;
            }

            StartCoroutine(SpinAttack());
        }

        private IEnumerator SpinAttack()
        {
            _isAttacking = true;

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
            _isAttacking = false;
        }
    }
}
