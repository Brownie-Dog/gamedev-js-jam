using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace ItemDrops
{
    public class ItemChoiceScreen : MonoBehaviour
    {
        [SerializeField]
        private GameObject _choiceScreenUiParent;

        private float _unpausedTimeScale = 1;

        private void Awake()
        {
            Assert.IsNotNull(_choiceScreenUiParent);
        }

        public void Show()
        {
            _choiceScreenUiParent.SetActive(true);

            Time.timeScale = 0;
        }

        private void Hide()
        {
            _choiceScreenUiParent.SetActive(false);

            Time.timeScale = 1;
        }

        public void OnChoicePicked(Action onChoicePicked)
        {
            onChoicePicked();

            Hide();
        }
    }
}
