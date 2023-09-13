using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DialogueScript", order = 1)]
public class DialogueScript : ScriptableObject
{
    public List<string> dialogueText;
}
