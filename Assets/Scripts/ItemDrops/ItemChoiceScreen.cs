using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ItemDrops
{
    public class ItemChoiceScreen : MonoBehaviour
    {
        [SerializeField]
        private GameObject _choiceScreenUiParent;

        [SerializeField]
        private Transform _cardContainer;

        [SerializeField]
        private UnityEngine.UI.Button _rerollButton;

        [SerializeField]
        private int _maxRerolls = 1;

        [SerializeField]
        private ItemCardButton _rareCardPrefab;

        [SerializeField]
        private ItemCardButton _epicCardPrefab;

        [SerializeField]
        private ItemCardButton _legendaryCardPrefab;

        public event EventHandler<ItemPickedEventArgs> ItemPicked;
        public event EventHandler RerollRequested;

        private int _rerollsRemaining;
        private List<ItemCardButton> _currentCards = new();

        private void Awake()
        {
            Assert.IsNotNull(_choiceScreenUiParent);
            Assert.IsNotNull(_cardContainer);
            Assert.IsNotNull(_rerollButton);
            Assert.IsNotNull(_rareCardPrefab);
            Assert.IsNotNull(_epicCardPrefab);
            Assert.IsNotNull(_legendaryCardPrefab);
        }

        private void Start()
        {
            _rerollButton.onClick.AddListener(() => RerollRequested?.Invoke(this, EventArgs.Empty));
        }

        public void Show(ItemData[] items)
        {
            _rerollsRemaining = _maxRerolls;
            CreateCards(items);

            _rerollButton.gameObject.SetActive(_rerollsRemaining > 0);
            _choiceScreenUiParent.SetActive(true);
            Time.timeScale = 0;
        }

        public void Reroll(ItemData[] items)
        {
            Assert.IsTrue(_rerollsRemaining > 0);
            _rerollsRemaining--;
            _rerollButton.gameObject.SetActive(_rerollsRemaining > 0);

            CreateCards(items);
        }

        private void Hide()
        {
            _choiceScreenUiParent.SetActive(false);
            Time.timeScale = 1;
        }

        public void OnChoicePicked(ItemData item)
        {
            ItemPicked?.Invoke(this, new ItemPickedEventArgs(item));
            Hide();
        }

        private void CreateCards(ItemData[] items)
        {
            ClearCards();

            foreach (var item in items)
            {
                if (item == null)
                {
                    continue;
                }

                var prefab = GetCardPrefab(item.Rarity);
                var card = Instantiate(prefab, _cardContainer);
                card.SetItemData(item);
                _currentCards.Add(card);
            }
        }

        private void ClearCards()
        {
            foreach (var card in _currentCards)
            {
                Destroy(card.gameObject);
            }

            _currentCards.Clear();
        }

        private ItemCardButton GetCardPrefab(Rarity rarity)
        {
            return rarity switch
            {
                Rarity.Legendary => _legendaryCardPrefab,
                Rarity.Epic => _epicCardPrefab,
                _ => _rareCardPrefab,
            };
        }
}
}
