using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleCharacter : BattleCharacter
{

    public PlayerHealthBarUI playerHealthBar;

    public override void UpdateStats(bool noFade = false)
    {
        base.UpdateStats();
        playerHealthBar.UpdateHealth(HP, maxHP, noFade);
        playerHealthBar.UpdateTexts(HP, AP, nameCharacter);
        playerHealthBar.UpdateAP(AP, maxAP, noFade);
    }

    public override void TurnStartReset()
    {
        base.TurnStartReset();
        animator.SetBool("TargetSelected", false);
    }

    public override void Defend()
    {
        base.Defend();
        animator.SetBool("Defending", true);
    }

    public override void Die()
    {
        base.Die();
        _engine.SetupAfterPlayerDeath(this);
    }
}
