using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class EnemyHealthBarUI : MonoBehaviour
{
    public List<BattleCharacter> owner;
    public List<Image> healthBars;
    public List<Image> healthBarsRed;
    public TMP_Text healthBarName;
    public int enemyAmount;

    public void UpdateHealth(int index, int HP, int maxHP, bool noFade = false)
    {
        float percentage = UIHelper.HealthBarPercent(HP, maxHP);
        healthBars[index].fillAmount = percentage;
        if (noFade)
        {
            healthBarsRed[index].fillAmount = percentage;
            return;
        }
        //healthBarsRed[index].DOFillAmount(percentage, 1f);
        healthBarsRed[index].DOFillAmount(percentage, 1f).OnComplete(owner[index].Die);
    }
}
