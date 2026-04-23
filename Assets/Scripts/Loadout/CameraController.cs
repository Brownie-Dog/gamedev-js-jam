using Player;
using UnityEngine;
using UnityEngine.Assertions;
using Weapons;

namespace Loadout
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float _zoomSpeed = 3f;

        [SerializeField] private float _zoomPadding = 1.5f;

        [SerializeField] private float _loadoutCenterOffsetX = -3f;

        [SerializeField] private PlayerEquipment _playerEquipment;

        [SerializeField] private float _shopFocusZoomMultiplier = 0.75f;

        private Camera _camera;
        private float _defaultOrthographicSize;
        private float _targetOrthographicSize;
        private Vector3 _defaultLocalPosition;
        private float _targetLocalX;
        private float _targetLocalY;
        private float _shopFocusZoomSize;
        private bool _loadoutOffsetActive;
        private bool _shopFocusActive;
        private Vector2 _shopFocusWorldPosition;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            Assert.IsNotNull(_camera);
            Assert.IsNotNull(_playerEquipment);

            _defaultOrthographicSize = _camera.orthographicSize;
            _targetOrthographicSize = _defaultOrthographicSize;
            _defaultLocalPosition = _camera.transform.localPosition;
            _targetLocalX = _defaultLocalPosition.x;
            _targetLocalY = _defaultLocalPosition.y;
        }

        // TODO: Call this on specific events instead of every frame
        private void LateUpdate()
        {
            var bounds = CalculatePlayerBounds();
            UpdateTargetOrthographicSize(bounds);
            UpdateTargetLocalPosition();

            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetOrthographicSize,
                _zoomSpeed * Time.deltaTime
            );

            var localPos = _camera.transform.localPosition;
            localPos.x = Mathf.Lerp(localPos.x, _targetLocalX, _zoomSpeed * Time.deltaTime);
            localPos.y = Mathf.Lerp(localPos.y, _targetLocalY, _zoomSpeed * Time.deltaTime);
            _camera.transform.localPosition = localPos;
        }

        public void SetLoadoutOffset(bool active)
        {
            _loadoutOffsetActive = active;
        }

        public void SetShopFocus(Vector2 worldPosition)
        {
            _shopFocusActive = true;
            _shopFocusWorldPosition = worldPosition;
            _shopFocusZoomSize = _defaultOrthographicSize * _shopFocusZoomMultiplier;
        }

        public void ClearShopFocus()
        {
            _shopFocusActive = false;
        }

        private void UpdateTargetOrthographicSize(Bounds bounds)
        {
            if (_shopFocusActive)
            {
                _targetOrthographicSize = _shopFocusZoomSize;
                return;
            }

            var aspect = _camera.aspect;
            var requiredSize = Mathf.Max(bounds.size.x / aspect, bounds.size.y) / 2f + _zoomPadding;

            if (_loadoutOffsetActive)
            {
                _targetOrthographicSize = requiredSize;
            }
            else
            {
                _targetOrthographicSize = Mathf.Max(requiredSize, _defaultOrthographicSize);
            }
        }

        private void UpdateTargetLocalPosition()
        {
            if (_shopFocusActive)
            {
                var playerWorldPos = _playerEquipment.transform.position;
                var targetWorldPos = new Vector3(
                    (playerWorldPos.x + _shopFocusWorldPosition.x) / 2f,
                    (playerWorldPos.y + _shopFocusWorldPosition.y) / 2f,
                    0f
                );
                var localOffset = _camera.transform.parent.InverseTransformPoint(targetWorldPos);
                _targetLocalX = localOffset.x;
                _targetLocalY = localOffset.y;
                return;
            }

            if (_loadoutOffsetActive)
            {
                _targetLocalX = _defaultLocalPosition.x + _loadoutCenterOffsetX;
                _targetLocalY = _defaultLocalPosition.y;
            }
            else
            {
                _targetLocalX = _defaultLocalPosition.x;
                _targetLocalY = _defaultLocalPosition.y;
            }
        }

        private Bounds CalculatePlayerBounds()
        {
            var markers = _playerEquipment.GetComponentsInChildren<IncludeInCameraBounds>();
            var hookPoints = _playerEquipment.GetComponentsInChildren<HookPoint>();

            var center = _playerEquipment.transform.position;
            var bounds = new Bounds(center, Vector3.one);

            bool hasAnyBounds = false;

            foreach (var marker in markers)
            {
                var renderer = marker.GetComponent<Renderer>();
                if (renderer != null)
                {
                    if (!hasAnyBounds)
                    {
                        bounds = renderer.bounds;
                        hasAnyBounds = true;
                    }
                    else
                    {
                        bounds.Encapsulate(renderer.bounds);
                    }
                }
            }

            foreach (var hookPoint in hookPoints)
            {
                if (!hasAnyBounds)
                {
                    bounds = new Bounds(hookPoint.transform.position, Vector3.one * 0.5f);
                    hasAnyBounds = true;
                }
                else
                {
                    bounds.Encapsulate(hookPoint.transform.position);
                }
            }

            return bounds;
        }
    }
}