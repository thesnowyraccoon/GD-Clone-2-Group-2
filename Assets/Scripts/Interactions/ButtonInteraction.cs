using UnityEngine;

public class ButtonInteraction : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Button has been interacted with");
    }
}
