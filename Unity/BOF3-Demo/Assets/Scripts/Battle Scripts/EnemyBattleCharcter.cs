using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattleCharacter : BattleCharacter
{
    public EnemyHealthBarUI healthBarUI;
    public int enemyID { get; set; }

    public override void Init()
    {
        base.Init();
        SetupEnemyFirstTime();
    }

    public void SetupEnemyFirstTime()
    {
        HP = maxHP;
        AP = maxAP;
    }

    public override void Defend()
    {
        base.Defend();
        BattleUI.instance.PulseBattleInfo(nameCharacter + " defends...");
    }

    public override void UpdateStats(bool noFade = false)
    {
        base.UpdateStats();
        healthBarUI.UpdateHealth(enemyID,HP, maxHP, this, noFade);
    }

    public override void Die()
    {
        if (HP > 0)
        {
            return;
        }
        base.Die();
        _engine.SetupAfterEnemyDeath(this);
    }


}
