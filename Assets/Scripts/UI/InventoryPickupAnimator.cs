using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI
{
    public class InventoryPickupAnimator : MonoBehaviour
    {
        public static InventoryPickupAnimator Instance { get; private set; }

        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _backpackIcon;
        [SerializeField] private GameObject _backpackRoot;
        [SerializeField] private float _duration = 0.75f;
        [SerializeField] private AnimationCurve _positionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve _scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private void Awake()
        {
            Assert.IsNull(Instance, "InventoryPickupAnimator.Instance already set!");
            Instance = this;

            Assert.IsNotNull(_canvas, "_canvas must be assigned in InventoryPickupAnimator.");
            Assert.IsNotNull(_backpackIcon, "_backpackIcon must be assigned in InventoryPickupAnimator.");
            Assert.IsNotNull(_backpackRoot, "_backpackRoot must be assigned in InventoryPickupAnimator.");
        }

        public void AnimatePickup(Sprite sprite, Vector2 startScreenPosition)
        {
            _backpackRoot.SetActive(true);

            GameObject flyingItemObj = new GameObject("FlyingItemSprite");
            RectTransform flyingItemRect = flyingItemObj.AddComponent<RectTransform>();
            flyingItemRect.SetParent(_canvas.transform, worldPositionStays: false);

            flyingItemRect.anchorMin = Vector2.zero;
            flyingItemRect.anchorMax = Vector2.zero;
            flyingItemRect.pivot = new Vector2(0.5f, 0.5f);

            Image flyingItemImage = flyingItemObj.AddComponent<Image>();
            flyingItemImage.sprite = sprite;

            flyingItemRect.anchoredPosition = startScreenPosition;
            flyingItemRect.localScale = Vector3.one;

            StartCoroutine(FlyAndHide(flyingItemRect));
        }

        private IEnumerator FlyAndHide(RectTransform flyingItem)
        {
            Vector2 startPos = flyingItem.anchoredPosition;
            Vector2 endPos = _backpackIcon.anchoredPosition;

            float elapsed = 0f;

            while (elapsed < _duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / _duration;

                flyingItem.anchoredPosition = Vector2.Lerp(startPos, endPos, _positionCurve.Evaluate(t));
                float scale = Mathf.Lerp(1f, 0.2f, _scaleCurve.Evaluate(t));
                flyingItem.localScale = Vector3.one * scale;

                yield return null;
            }

            Destroy(flyingItem.gameObject);

            yield return new WaitForSecondsRealtime(0.5f);
            _backpackRoot.SetActive(false);
        }
    }
}
