using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

namespace Generic
{
    /// <summary>
    /// Updates sorting order every frame based on world Y position so that
    /// sprites higher on the screen (further back) render behind sprites
    /// lower on the screen (closer to the camera).
    /// </summary>
    public class SpriteDepthSorter : MonoBehaviour
    {
        [SerializeField] private bool _includeChildren = true;
        [SerializeField] private int _baseOrderOffset = 0;
        [Tooltip("Multiplier for Y-to-sorting-order conversion. Increase if objects are very close together in Y.")]
        [SerializeField] private int _yResolution = 100;

        private SpriteRenderer[] _spriteRenderers;
        private SortingGroup[] _sortingGroups;

        private void Awake()
        {
            RefreshRenderers();
        }

        private void LateUpdate()
        {
            int order = ComputeOrder();

            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                _spriteRenderers[i].sortingOrder = order;
            }

            for (int i = 0; i < _sortingGroups.Length; i++)
            {
                _sortingGroups[i].sortingOrder = order;
            }
        }

        public void RefreshRenderers()
        {
            if (_includeChildren)
            {
                _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
                _sortingGroups = GetComponentsInChildren<SortingGroup>(true);
            }
            else
            {
                _spriteRenderers = new[] { GetComponent<SpriteRenderer>() };
                _sortingGroups = new[] { GetComponent<SortingGroup>() };
            }
        }

        private int ComputeOrder()
        {
            return _baseOrderOffset - Mathf.RoundToInt(transform.position.y * _yResolution);
        }
    }
}
