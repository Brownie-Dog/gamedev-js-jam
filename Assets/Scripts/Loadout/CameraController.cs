using Player;
using UnityEngine;
using Weapons;

namespace Loadout
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private float _zoomSpeed = 3f;

        [SerializeField]
        private float _zoomPadding = 1.5f;

        [SerializeField]
        private float _loadoutCenterOffsetX = -3f;

        [SerializeField]
        private PlayerEquipment _playerEquipment;

        private Camera _camera;
        private float _defaultOrthographicSize;
        private float _targetOrthographicSize;
        private Vector3 _defaultLocalPosition;
        private float _targetLocalX;
        private bool _loadoutOffsetActive;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _defaultOrthographicSize = _camera.orthographicSize;
            _targetOrthographicSize = _defaultOrthographicSize;
            _defaultLocalPosition = _camera.transform.localPosition;
            _targetLocalX = _defaultLocalPosition.x;
        }

        private void LateUpdate()
        {
            var bounds = CalculatePlayerBounds();
            UpdateTargetOrthographicSize(bounds);
            UpdateTargetLocalX();

            _camera.orthographicSize = Mathf.Lerp(
                _camera.orthographicSize,
                _targetOrthographicSize,
                _zoomSpeed * Time.deltaTime
            );

            var localPos = _camera.transform.localPosition;
            localPos.x = Mathf.Lerp(localPos.x, _targetLocalX, _zoomSpeed * Time.deltaTime);
            _camera.transform.localPosition = localPos;
        }

        public void SetLoadoutOffset(bool active)
        {
            _loadoutOffsetActive = active;
        }

        private void UpdateTargetOrthographicSize(Bounds bounds)
        {
            var aspect = _camera.aspect;
            var requiredSize = Mathf.Max(
                bounds.size.x / aspect,
                bounds.size.y
            ) / 2f + _zoomPadding;

            if (_loadoutOffsetActive)
            {
                _targetOrthographicSize = requiredSize;
            }
            else
            {
                _targetOrthographicSize = Mathf.Max(requiredSize, _defaultOrthographicSize);
            }
        }

        private void UpdateTargetLocalX()
        {
            if (_loadoutOffsetActive)
            {
                _targetLocalX = _defaultLocalPosition.x + _loadoutCenterOffsetX;
            }
            else
            {
                _targetLocalX = _defaultLocalPosition.x;
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