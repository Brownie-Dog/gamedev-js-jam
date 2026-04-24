using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

namespace Dialogue
{
    public class DialogueSpeaker : MonoBehaviour
    {
        [SerializeField] private Dialogue _dialogue;
        [SerializeField] private GameObject _speechBubblePrefab;
        [SerializeField] private Transform _speakerTransform;
        [SerializeField] private Collider2D _detectionCollider;
        [SerializeField] private float _xOffset;
        [SerializeField] private float _yOffset = 1f;
        [SerializeField] private bool _oneShot = true;

        private Transform _bubbleTransform;
        private TextMeshPro _dialogueText;
        private Coroutine _dialogueCoroutine;
        private bool _hasPlayed;
        private bool _isActive;

        private void Awake()
        {
            Assert.IsNotNull(_dialogue);
            Assert.IsNotNull(_speechBubblePrefab);
            Assert.IsNotNull(_speakerTransform);

            if (_detectionCollider != null)
            {
                Assert.IsNotNull(_detectionCollider);
            }
        }

        private void Update()
        {
            if (_isActive && _bubbleTransform != null)
            {
                _bubbleTransform.position = _speakerTransform.position + new Vector3(_xOffset, _yOffset, 0);
                _bubbleTransform.rotation = Quaternion.identity;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(GlobalConstants.PLAYER_TAG))
            {
                TryStartDialogue();
            }
        }

        public void TryStartDialogue()
        {
            if (_oneShot && _hasPlayed)
            {
                return;
            }

            StartDialogue();
            _hasPlayed = true;
        }

        public void StartDialogue()
        {
            if (_bubbleTransform == null)
            {
                var bubble = Instantiate(_speechBubblePrefab);
                _bubbleTransform = bubble.transform;
                _dialogueText = bubble.GetComponentInChildren<TextMeshPro>();
                Assert.IsNotNull(_dialogueText);
            }

            _isActive = true;
            _bubbleTransform.gameObject.SetActive(true);

            if (_dialogueCoroutine != null)
            {
                StopCoroutine(_dialogueCoroutine);
            }

            _dialogueCoroutine = StartCoroutine(PlayDialogue());
        }

        public void StopDialogue()
        {
            _isActive = false;

            if (_dialogueCoroutine != null)
            {
                StopCoroutine(_dialogueCoroutine);
                _dialogueCoroutine = null;
            }

            if (_bubbleTransform != null)
            {
                _bubbleTransform.gameObject.SetActive(false);
            }
        }

        public void ResetOneShot()
        {
            _hasPlayed = false;
        }

        private IEnumerator PlayDialogue()
        {
            var entries = _dialogue.Entries;

            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                _dialogueText.text = entry.Text;

                yield return new WaitForSeconds(entry.Duration);
            }

            StopDialogue();
        }
    }
}