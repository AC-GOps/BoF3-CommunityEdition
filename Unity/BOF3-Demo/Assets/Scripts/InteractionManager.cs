using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    private Narrator _Narrator;

    public InteractableObject interactableObject;

    public bool canInteract;

    private void Start()
    {
        _Narrator = Narrator.Instance;
    }

    public void Update()
    {
        GetInteractionInput();
    }

    private void GetInteractionInput()
    {
        if(!canInteract)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if(interactableObject == null)
            {
                return ;
            }

            _Narrator.script = interactableObject.used ? interactableObject.scriptUsed : interactableObject.script;
            _Narrator.OpenTextBox();
            interactableObject.used = true;
        }
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
