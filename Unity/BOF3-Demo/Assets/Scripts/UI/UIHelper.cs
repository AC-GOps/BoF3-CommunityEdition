using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHelper
{ 
    public static float HealthBarPercent(int _HP, int _maxHP)
    {
        float healthPercent = (float)_HP / _maxHP;
        return healthPercent;
    }

}
