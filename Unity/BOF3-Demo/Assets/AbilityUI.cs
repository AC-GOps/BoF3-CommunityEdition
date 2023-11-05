using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityUI : MonoBehaviour
{
    public TMP_Text skillName;
    public TMP_Text apCost;
    public Image element;
    public Ability ability;
    public Color disabledColor;

    public void DisableAbility()
    {
        skillName.color = disabledColor;
        apCost.color = disabledColor;
        element.color = disabledColor;
    }

    public void EnableAbility()
    {
        skillName.color = Color.white;
        apCost.color = Color.white;
        element.color = Color.white;
    }
}
