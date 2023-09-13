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

    public override void UpdateStats(bool noFade = false)
    {
        healthBarUI.UpdateHealth(ammount,HP, maxHP, noFade);
    }

    public override void Die()
    {
        base.Die();
        _engine.SetupAfterEnemyDeath(this);
    }


}
