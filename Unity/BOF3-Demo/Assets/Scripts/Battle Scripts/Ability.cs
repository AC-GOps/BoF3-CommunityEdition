using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Ability", order = 1)]
public class Ability : ScriptableObject
{
    public string abilityName;
    public AbilityType type;
    public AbilityElement element;
    public int strength;
    public int apCost;
    public string abilityDes;
    public float castTime;
    public bool targetAll;

}
