using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Enemy.Bosses
{
    public class OvenBossArm : MonoBehaviour
    {
        [SerializeField] private Transform _elbow;
        [SerializeField] private Transform _segmentsRoot;
        [SerializeField] private GameObject _segmentPrefab;
        [SerializeField] private GameObject _defaultHandPrefab;
        [SerializeField] private int _defaultSegmentCount = 3;
        [SerializeField] private float _segmentSpacing = 1.87f;
        [SerializeField] private float _handOffset = -1f;
        [SerializeField] private float _extendSpeed = 0.05f;
        [SerializeField] private float _retractSpeed = 0.03f;

        private readonly List<GameObject> _segments = new List<GameObject>();
        private GameObject _hand;
        private Coroutine _extendRoutine;
        private Coroutine _retractRoutine;

        public bool IsExtending => _extendRoutine != null;
        public bool IsRetracting => _retractRoutine != null;
        public bool IsMoving => IsExtending || IsRetracting;
        public int SegmentCount => _segments.Count;
        public Transform HandPosition => _hand != null ? _hand.transform : null;
        public int DefaultSegmentCount => _defaultSegmentCount;
        public GameObject DefaultHandPrefab => _defaultHandPrefab;

        private void Start()
        {
            Assert.IsNotNull(_elbow);
            Assert.IsNotNull(_segmentsRoot);
            Assert.IsNotNull(_segmentPrefab);
            Assert.IsNotNull(_defaultHandPrefab);

            Initialize(_defaultSegmentCount, _defaultHandPrefab);
        }

        public void Initialize(int segmentCount, GameObject handPrefab)
        {
            ClearArm();

            for (int i = 0; i < segmentCount; i++)
            {
                SpawnSegment();
            }

            SpawnHand(handPrefab);
            _segmentsRoot.gameObject.SetActive(true);
        }

        public void SetDirection(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        public void ExtendTo(int targetCount)
        {
            if (_extendRoutine != null)
            {
                StopCoroutine(_extendRoutine);
            }

            _extendRoutine = StartCoroutine(ExtendRoutine(targetCount));
        }

        public void RetractToDefault()
        {
            if (_retractRoutine != null)
            {
                StopCoroutine(_retractRoutine);
            }

            _retractRoutine = StartCoroutine(RetractRoutine(_defaultSegmentCount));
        }

        public void SwapHand(GameObject newHandPrefab)
        {
            Assert.IsNotNull(newHandPrefab);

            Vector3 position = _hand.transform.localPosition;
            Destroy(_hand);
            _hand = Instantiate(newHandPrefab, _segmentsRoot);
            _hand.transform.localPosition = position;
        }

        private void SpawnSegment()
        {
            int index = _segments.Count;
            var segment = Instantiate(_segmentPrefab, _segmentsRoot);
            segment.transform.localPosition = new Vector3(0f, -index * _segmentSpacing, 0f);
            _segments.Add(segment);
        }

        private void SpawnHand(GameObject handPrefab)
        {
            _hand = Instantiate(handPrefab, _segmentsRoot);
            _hand.transform.localPosition = new Vector3(0f, -_segments.Count * _segmentSpacing + _handOffset, 0f);
        }

        private void ClearArm()
        {
            foreach (var segment in _segments)
            {
                Destroy(segment);
            }

            _segments.Clear();

            if (_hand != null)
            {
                Destroy(_hand);
            }
        }

        private IEnumerator ExtendRoutine(int targetCount)
        {
            while (_segments.Count < targetCount)
            {
                SpawnSegment();
                RepositionHand();
                yield return new WaitForSeconds(_extendSpeed);
            }

            _extendRoutine = null;
        }

        private IEnumerator RetractRoutine(int targetCount)
        {
            while (_segments.Count > targetCount)
            {
                GameObject last = _segments[_segments.Count - 1];
                _segments.RemoveAt(_segments.Count - 1);
                Destroy(last);
                RepositionHand();
                yield return new WaitForSeconds(_retractSpeed);
            }

            _retractRoutine = null;
        }

        private void RepositionHand()
        {
            if (_hand != null)
            {
                _hand.transform.localPosition = new Vector3(0f, -_segments.Count * _segmentSpacing + _handOffset, 0f);
            }
        }
    }
}