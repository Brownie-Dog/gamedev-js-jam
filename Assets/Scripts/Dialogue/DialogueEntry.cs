using UnityEngine;

namespace Dialogue
{
    [System.Serializable]
    public class DialogueEntry
    {
        [SerializeField] private string _text;
        [SerializeField] private float _duration = 3f;

        public string Text => _text;
        public float Duration => _duration;
    }
}