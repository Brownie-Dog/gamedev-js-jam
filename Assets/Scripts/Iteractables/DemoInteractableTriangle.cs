using UnityEngine;

public class DemoInteractableTriangle : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        UnityEngine.Debug.Log($"You have interacted with the triangle - {gameObject.name}");
    }
}
