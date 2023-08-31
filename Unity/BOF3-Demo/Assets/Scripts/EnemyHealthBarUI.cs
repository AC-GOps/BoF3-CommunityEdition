using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthBarUI : MonoBehaviour
{
    public List<BattleCharacter> owner;
    public List<Image> healthBars;
    public List<Image> healthBarsRed;
    public TMP_Text healthBarName;
    public int enemyAmount;
}
