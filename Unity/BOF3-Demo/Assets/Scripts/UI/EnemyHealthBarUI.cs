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
    public List<Image> healthBarsParent;
    public TMP_Text healthBarName;
    public int enemyAmount;

    public void UpdateHealth(int index, int HP, int maxHP, BattleCharacter enemy, bool noFade = false)
    {
        float percentage = UIHelper.HealthBarPercent(HP, maxHP);
        healthBars[index].fillAmount = percentage;
        if (noFade)
        {
            healthBarsParent[index].fillAmount = percentage;
            return;
        }

        healthBarsParent[index].DOFillAmount(percentage, 1f).OnComplete(enemy.Die);   
    }

    public void Reset()
    {
        healthBarName.text = "";
        owner.Clear();
        enemyAmount = 0;
        gameObject.SetActive(false);
    }
}
