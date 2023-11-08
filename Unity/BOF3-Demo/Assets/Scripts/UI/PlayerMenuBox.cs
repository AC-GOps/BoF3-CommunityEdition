using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMenuBox : MonoBehaviour
{
    public TMP_Text playerName;
    public Image expBar;
    public TMP_Text playerlevel;
    public TMP_Text playerHP;
    public TMP_Text playerAP;

    public void UpdateInfo(PlayerBattleCharacter character)
    {
        playerName.text = character.nameCharacter;
        playerlevel.text = character.Level.ToString();
        playerHP.text = character.HP.ToString() + " / " + character.maxHP.ToString();
        playerAP.text = character.AP.ToString() + " / " + character.maxAP.ToString();
    }
}
