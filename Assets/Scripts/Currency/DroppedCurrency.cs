using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

namespace Currency
{
    public class DroppedCurrency : MonoBehaviour
    {
        public IObjectPool<DroppedCurrency> Pool { get; set; }

        public int Amount { get; set; } = 1;

        [SerializeField] private float _magnetRadius = 3f;

        [SerializeField] private float _collectRadius = 0.3f;

        [SerializeField] private float _magnetSpeed = 8f;

        [SerializeField] private float _spawnDelay = 0.5f;


        private Transform _playerTransform;
        private float _landTime;

        private enum CoinState
        {
            Spawning,
            Landed,
            Magnet,
        }

        private CoinState _state;

        private void Awake()
        {
            _playerTransform = GameObject.FindGameObjectWithTag(GlobalConstants.PLAYER_TAG).transform;
            Assert.IsNotNull(_playerTransform);
        }

        private void Update()
        {
            switch (_state)
            {
                case CoinState.Spawning:
                    break;

                case CoinState.Landed:
                    if (Time.time - _landTime >= _spawnDelay &&
                        Vector3.Distance(transform.position, _playerTransform.position) <= _magnetRadius)
                    {
                        _state = CoinState.Magnet;
                    }

                    break;

                case CoinState.Magnet:
                    transform.position = Vector3.Lerp(transform.position, _playerTransform.position,
                        _magnetSpeed * Time.deltaTime
                    );

                    if (Vector3.Distance(transform.position, _playerTransform.position) < _collectRadius)
                    {
                        Collect();
                    }

                    break;
            }
        }

        public void PlaySpawnAnimation(Vector3 from, Vector3 to)
        {
            // TODO: LitMotion arc/bounce tween from `from` to `to`
            transform.position = to;
            _state = CoinState.Landed;
            _landTime = Time.time;
        }

        private void Collect()
        {
            CurrencyManager.Instance.AddCurrency(Amount);
            Pool.Release(this);
        }
    }
}