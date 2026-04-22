using UnityEngine;
using UnityEngine.Assertions;

namespace Currency
{
    public class CurrencyManager : MonoBehaviour
    {
        public static CurrencyManager Instance { get; private set; }

        [SerializeField] private DroppedCurrency _currencyPrefab;

        [SerializeField] private PlayerStatsSo _playerStats;

        [SerializeField] private int _poolCapacity = 50;

        [SerializeField] private float _spreadRadius = 1f;

        private MonoBehaviourPool<DroppedCurrency> _pool;

        private void Awake()
        {
            Assert.IsNull(Instance);
            Instance = this;

            Assert.IsNotNull(_currencyPrefab);
            Assert.IsNotNull(_playerStats);

            _pool = new MonoBehaviourPool<DroppedCurrency>(_currencyPrefab, _poolCapacity, _poolCapacity, transform,
                coin => coin.Pool = _pool.Pool
            );
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void SpawnCurrency(Vector2 position, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var coin = _pool.Get();
                var offset = Random.insideUnitCircle * _spreadRadius;
                var targetPosition = position + offset;
                coin.transform.position = position;
                coin.PlaySpawnAnimation(position, targetPosition);
            }
        }

        public void AddCurrency(int amount)
        {
            _playerStats.AddCurrency(amount);
        }
    }
}