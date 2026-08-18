using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    bool inRange = false;

    IInteractable interactableInRange = null;

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed && inRange)
        {
            Debug.Log("Interacting");

            interactableInRange?.Interact();
        }
        else
        {
            return;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        inRange = collision.gameObject.TryGetComponent(out IInteractable interactable);

        if (inRange)
        {
            interactableInRange = interactable;

            Debug.Log("Interactable in range");
        }
    }

    void OnCollisionExit(Collision collision)
    {
        inRange = collision.gameObject.TryGetComponent(out IInteractable interactable);

        if (inRange && interactable == interactableInRange)
        {
            interactableInRange = null;

            Debug.Log("Interactable out of range");
        }
    }
}
