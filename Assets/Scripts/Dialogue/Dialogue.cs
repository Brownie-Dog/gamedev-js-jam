using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = nameof(Dialogue), menuName = "ScriptableObjects/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private List<DialogueEntry> _entries = new();

        public IReadOnlyList<DialogueEntry> Entries => _entries;
    }
}