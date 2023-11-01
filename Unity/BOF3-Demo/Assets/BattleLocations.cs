using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLocations : MonoBehaviour
{
    public List<Transform> Locations = new List<Transform>();

    private void Awake()
    {
        Locations.Clear();
        Locations.AddRange(GetComponentsInChildren<Transform>());
    }
}
