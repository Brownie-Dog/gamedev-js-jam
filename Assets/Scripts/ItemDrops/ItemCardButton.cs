using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace ItemDrops
{
    public class ItemCardButton : MonoBehaviour
    {
        private Button _itemCardButton;

        private ItemChoiceScreen _itemChoiceScreen;

        private void Awake()
        {
            _itemChoiceScreen = GetComponentInParent<ItemChoiceScreen>();
            Assert.IsNotNull(_itemChoiceScreen);

            _itemCardButton = GetComponent<Button>();
            Assert.IsNotNull(_itemCardButton);
        }

        private void Start()
        {
            _itemCardButton.onClick.AddListener(OnCardButtonPressed);
        }

        private void OnCardButtonPressed()
        {
            _itemChoiceScreen.OnChoicePicked(OnCardSelected);
        }

        private void OnCardSelected()
        {
            Debug.Log("OnCardSelected");
        }
    }
}
