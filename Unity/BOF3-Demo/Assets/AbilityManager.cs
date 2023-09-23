using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Abilites
{
    Flare,
    Cyclone,
    Frost
}

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager instance;
    public List<GameObject> abilityEffcts = new List<GameObject>();
    public Vector3 offest;

    private void Awake()
    {
        instance = this;
    }

    public void PlayEffect(Transform target, string abilityName)
    {
        Abilites ab = (Abilites)Enum.Parse(typeof(Abilites), abilityName);
        //Testing only
        Vector3 targetPos = target.position + offest;
        abilityEffcts[(int)ab].transform.position = targetPos;
        abilityEffcts[(int)ab].GetComponent<ParticleSystem>().Play();
    }
}
