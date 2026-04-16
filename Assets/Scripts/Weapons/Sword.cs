using UnityEngine;
using UnityEngine.Assertions;

namespace Weapons
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Sword : MonoBehaviour, IWeapon
    {
        [SerializeField] private BaseWeaponSo _weaponData;
        [SerializeField] private float _rotationSpeed = 180f;

        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            Assert.IsNotNull(_renderer);
        }

        private void Start()
        {
            _renderer.sprite = _weaponData.Sprite;
        }

        public void Attack()
        {
            Debug.Log("Sword attack!");
        }

        private void Update()
        {
            transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);
        }
    }
}