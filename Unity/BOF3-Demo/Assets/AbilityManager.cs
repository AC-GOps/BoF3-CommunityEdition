using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager instance;
    public List<GameObject> abilityEffcts = new List<GameObject>();
    public Vector3 offest;

    private void Awake()
    {
        instance = this;
    }

    public void PlayEffect(Transform target)
    {
        //Testing only
        target.position += offest;
        abilityEffcts[0].transform.position = target.position;
        abilityEffcts[0].GetComponent<ParticleSystem>().Play();
    }
}
