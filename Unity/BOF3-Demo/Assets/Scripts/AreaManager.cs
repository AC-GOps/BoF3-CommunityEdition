using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    public GameObject hideInBattle;
    public static AreaManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void HideGameObjects()
    {
        hideInBattle.SetActive(false);
    }    
    public void ShowGameObjects()
    {
        hideInBattle.SetActive(true);
    }
}
