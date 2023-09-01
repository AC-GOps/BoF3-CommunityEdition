using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PlayerScript requires the GameObject to have a BoxCollider component
[RequireComponent(typeof(BoxCollider))]
public class InteractableObject : MonoBehaviour
{
    public DialogueScript script;
    public DialogueScript scriptUsed;
    public bool used;

    //public bool triggerAction;
    public bool battleTriggerTest;

    // add item
}
