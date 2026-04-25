using UnityEngine;
using UnityEngine.Assertions;

namespace Weapons
{
    public class SprinklerBullet : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Sprite[] _possibleSprites;

        private void Awake()
        {
            Assert.IsNotNull(_renderer);
            Assert.IsTrue(_possibleSprites.Length > 0);
        }

        private void Start()
        {
            _renderer.sprite = _possibleSprites[Random.Range(0, _possibleSprites.Length)];
        }
    }
}