using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace UI
{
    public class CurrencyUI : MonoBehaviour
    {
        [SerializeField] private PlayerStatsSo _playerStats;
        [SerializeField] private TextMeshProUGUI _currencyText;

        private void Awake()
        {
            Assert.IsNotNull(_playerStats);
            Assert.IsNotNull(_currencyText);
        }

        private void Update()
        {
            _currencyText.SetText(_playerStats.Currency.ToString());
        }
    }
}
