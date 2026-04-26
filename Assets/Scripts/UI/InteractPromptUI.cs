using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class InteractPromptUI : MonoBehaviour
{
    [SerializeField] private PlayerInteractionController _interactionController;
    [SerializeField] private TextMeshProUGUI _promptText;

    private void Awake()
    {
        Assert.IsNotNull(_interactionController);
        Assert.IsNotNull(_promptText);
    }

    private void OnEnable()
    {
        _interactionController.OnInteractableInRange += HandleInteractableChanged;
    }

    private void OnDisable()

    {
        _interactionController.OnInteractableInRange -= HandleInteractableChanged;
    }

    private void HandleInteractableChanged(bool inRange)
    {
        _promptText.enabled = inRange;
    }
}
