using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PlayerHealthBarUI : MonoBehaviour
{
    public Image healthBar;
    public Image healthBarRed;
    public Image apBar;
    public Image apBarRed;
    public TMP_Text healthBarName;
    public TMP_Text healthInt;
    public TMP_Text apInt;

    public void UpdateTexts(int health, int ap, string name)
    {
        healthInt.text = health.ToString();
        apInt.text = ap.ToString();
        healthBarName.text = name;
    }

    public void UpdateHealth(int HP, int maxHP, bool noFade = false)
    {
        float percentage = UIHelper.HealthBarPercent(HP, maxHP);
        healthBar.fillAmount = percentage;
        if (noFade)
        {
            healthBarRed.fillAmount = percentage;
            return;
        }
        healthBarRed.DOFillAmount(percentage, 1f);
    }

    public void UpdateAP(int AP, int maxAP, bool noFade = false)
    {
        float percentage = UIHelper.HealthBarPercent(AP, maxAP);
        apBar.fillAmount = percentage;
        if (noFade)
        {
            apBarRed.fillAmount = percentage;
            return;
        }
        apBarRed.DOFillAmount(percentage, 1f);
    }

}
