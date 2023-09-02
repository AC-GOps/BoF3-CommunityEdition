using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattleCharacter : BattleCharacter
{
    public EnemyHealthBarUI healthBarUI;
    public int ammount;

    public override void Defend()
    {
        base.Defend();
        BattleUI.instance.PulseBattleInfo(nameCharacter + " defends...");
    }

    public override void UpdateHealth(bool noFade = false)
    {
        healthBarUI.UpdateHealth(ammount,HP, maxHP, noFade);
    }

    public override void Die()
    {
        _engine.SetupAfterEnemyDeath(this);
    }


}
