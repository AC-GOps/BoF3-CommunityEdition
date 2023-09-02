using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleCharacter : BattleCharacter
{

    public PlayerHealthBarUI playerHealthBar;

    public override void UpdateHealth(bool noFade = false)
    {
        playerHealthBar.UpdateHealth(HP, maxHP, noFade);
        playerHealthBar.UpdateTexts(HP, AP, nameCharacter);
    }
}
