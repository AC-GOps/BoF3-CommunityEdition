using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// PlayerScript requires the GameObject to have a BoxCollider component
[RequireComponent(typeof(BoxCollider))]
public class InteractableObject : MonoBehaviour
{
    public DialogueScript script;
    public DialogueScript scriptUsed;
    public bool used;
    public UnityEvent onClose; 
    public UnityEvent onOpen; 

    public bool battleTriggerTest;

}
