using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattleCharacter : BattleCharacter
{
    public EnemyHealthBarUI healthBarUI;
    public int ammount;
    private EnemyBattleCharacter enemyBattleCharacter;

    public EnemyBattleCharacter(EnemyBattleCharacter enemyBattleCharacter)
    {
        this.enemyBattleCharacter = enemyBattleCharacter;
    }

    public override void Init()
    {
        base.Init();
        SetupEnemyFirstTime();
    }

    public void SetupEnemyFirstTime()
    {
        HP = maxHP;
        Defence = baseDefence;
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
        healthBarUI.UpdateHealth(ammount,HP, maxHP, noFade);
    }

    public override void Die()
    {
        base.Die();
        _engine.SetupAfterEnemyDeath(this);
    }


}
