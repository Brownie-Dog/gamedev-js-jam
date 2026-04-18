using System;
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
        private float _rotationSpeed = 180f;

        private SpriteRenderer _renderer;

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

        private void Update()
        {
            transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);
        }

        protected override void OnSecondaryAttack(object obj, EventArgs args)
        {
            Attack();
        }

        public void Attack()
        {
            Debug.Log("Sword attack!");
        }
    }
}
