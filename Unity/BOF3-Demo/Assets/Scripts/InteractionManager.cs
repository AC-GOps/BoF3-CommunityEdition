using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    public Narrator _Narrator;

    public InteractableObject interactableObject;

    public bool canInteract;

    private void Start()
    {
        _Narrator = Narrator.Instance;
    }

    public void GetInteractionInput(InputAction.CallbackContext context)
    {
        if(!canInteract)
        {
            return;
        }

        if (interactableObject == null)
        {
            return;
        }

        if (context.performed)
        {
            _Narrator.script = interactableObject.used ? interactableObject.scriptUsed : interactableObject.script;
            _Narrator.OpenTextBox();
            interactableObject.used = true;
            canInteract = false;
            PlayerInputManager.Instance.SwapActionMaps("Narration");
        }
    }

    public void CallDealyedCanInteract()
    {
        StartCoroutine(DealyedCanInteract());
    }

    public IEnumerator DealyedCanInteract()
    {
        canInteract = false;
        yield return new WaitForSeconds(2);
        canInteract = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            canInteract = true;
            interactableObject = other.GetComponent<InteractableObject>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            canInteract = false;
            interactableObject = null;
        }
    }
}
