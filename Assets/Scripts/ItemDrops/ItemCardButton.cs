using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace ItemDrops
{
    public class ItemCardButton : MonoBehaviour
    {
        [SerializeField]
        private Button _itemCardButton;

        [SerializeField]
        private TMPro.TextMeshProUGUI _titleText;

        [SerializeField]
        private TMPro.TextMeshProUGUI _descriptionText;

        [SerializeField]
        private Image _iconImage;

        private ItemData _itemData;
        private ItemChoiceScreen _itemChoiceScreen;

        private void Awake()
        {
            _itemChoiceScreen = GetComponentInParent<ItemChoiceScreen>();
            Assert.IsNotNull(_itemChoiceScreen);

            Assert.IsNotNull(_itemCardButton);
            Assert.IsNotNull(_titleText);
            Assert.IsNotNull(_descriptionText);
            Assert.IsNotNull(_iconImage);
        }

        private void Start()
        {
            _itemCardButton.onClick.AddListener(OnCardButtonPressed);
        }

        public void SetItemData(ItemData itemData)
        {
            _itemData = itemData;

            _titleText.text = itemData.ItemName;
            _descriptionText.text = itemData.Description;
            _iconImage.sprite = itemData.Icon;
        }

        private void OnCardButtonPressed()
        {
            Assert.IsNotNull(_itemData);
            _itemChoiceScreen.OnChoicePicked(_itemData);
        }
    }
}
