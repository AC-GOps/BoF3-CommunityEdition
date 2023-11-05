using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum BattleActionType
{
    Attack,
    Defend,
    Ability,
    Default
}

[System.Serializable]
public class BattleResitances
{
    public AbilityElement abilityType;
    public int Level;
}

public class BattleCharacter : MonoBehaviour
{
    protected TurnBasedBattleEngine _engine;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public List<BattleCharacter> targets;
    [HideInInspector]
    public BattleActionType battleActionType;
    public string nameCharacter;
    public int HP, maxHP;
    public int Defence, baseDefence;
    public int AP, maxAP;
    public int Agility;
    public int Level;
    public int Dodge;
    public int Drop;
    public int DeathStat;
    public int Power;
    public int Wisdom;
    public int PsionicStatChance;
    public int Reprisal;
    public List<BattleResitances> Resistances = new List<BattleResitances>(); // Res against -100% - 300% Flame, Frost, Electric, Earth & Wind / Psionic, Status, Death -200% - 200%
    public List<AudioClip> attackSounds;
    public List<AudioClip> hurtSounds;

    public List<Ability> battleAbilities;
    public Ability activeAbility;
    public bool takenAction { get; set; }
    public NumberBouncer numBouncer;



    public void CalculateRes(AbilityElement type, int damage)
    {
        BattleResitances res = new BattleResitances();

        res = Resistances.Find(i => i.abilityType == type);

        if (res == null)
        {
            print("No Res Found acting as level 2 = 100%");
            res = new BattleResitances();
            res.Level = 2;
        }

        switch (res.Level)
        {
            case 0:
                damage = damage * 3;
                break;
            case 1:
                damage = damage * 2;
                break;
            case 2:
                break;
            case 3:
                damage = (int)(damage * 0.75f);
                break;
            case 4:
                damage = (int)(damage * 0.5f);
                break;
            case 5:
                damage = (int)(damage * 0.25f);
                break;
            case 6:
                damage = 0;
                break;
            case 7:
                damage = damage * -1;
                break;
            default:
                break;
        }

        TakeMagicDamage(damage);
    }

    private void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        animator = GetComponentInChildren<Animator>();
        _engine = TurnBasedBattleEngine.Instance;
        Defence = baseDefence;
    }

    private void DodgeAttack(out int dam)
    {
        dam = 0;
        var chance = Random.Range(0, 100);
        if (chance < Dodge)
        {
            print(nameCharacter + " dodged attack");
            animator.SetTrigger("Dodge");
            dam = -1;
            numBouncer.PlayTextBounceAtTarget(transform, "Miss");
        }
    }

    private void TakeMagicDamage(int damage)
    {
        int actualDamage = damage - Wisdom;
        numBouncer.PlayNumberBounceAtTarget(transform, actualDamage);

        if (actualDamage == 0)
        {
            actualDamage = 0;
            print(nameCharacter + " took no damage");

            return;
        }

        HP -= actualDamage;
        UpdateStats();

        if (actualDamage < 0)
        {
            print(nameCharacter + " is healed by " + actualDamage + " damage");
            return;
        }

        PlayHurtSFX();
        print(nameCharacter + " took " + actualDamage + " damage");
        animator.SetTrigger("Hurt");

        if(HP>0)
        {
            return;
        }
        animator.SetBool("Die", true);
    }

    private void TakeDamage(int damage)
    {
        var dama =0;
        DodgeAttack(out dama);

        if(dama ==-1)
        {
            return;
        }

        int actualDamage = damage - Defence;
        if(actualDamage < 0)
        {
            actualDamage = 0;
        }
        numBouncer.PlayNumberBounceAtTarget(transform, actualDamage);

        if (actualDamage == 0)
        {
            print(nameCharacter + " took no damage");
            return;
        }

        PlayHurtSFX();

        HP -= actualDamage;
        print(nameCharacter + " took " + actualDamage + " damage");
        UpdateStats();
        animator.SetTrigger("Hurt");
        TurnBasedBattleEngine.Instance.PlayHitAnimation();

        if (HP > 0)
        {
            return;
        }
        animator.SetBool("Die", true);
    }

    public virtual void Die()
    {
        print(nameCharacter + " died");
    }

    public virtual void UpdateStats(bool noFade = false)
    {
        if (HP > maxHP)
        {
            HP = maxHP;
        }
        if (HP < 0)
        {
            HP = 0;
        }
        // called individually in child classes
    }

    public void AttemptAction()
    {
        if(takenAction)
        {
            print(nameCharacter + " has already taken action " + battleActionType.ToString());
            _engine.UpdateBattleEngine();
            return;
        }

        switch (battleActionType)
        {
            case BattleActionType.Attack:
                Attack();
                break;
            case BattleActionType.Defend:
                Defend();
                StartCoroutine(DelayedBattleEngineUpdate(2f));
                break;
            case BattleActionType.Ability:
                Ability();
                break;
            default:
                break;
        }

        takenAction = true;
    }

    public virtual void TurnStartReset()
    {
        if (battleActionType == BattleActionType.Defend)
        {
            StopDefend();
        }
        if(targets!=null)
        {
            targets.Clear();
        }

        activeAbility = null;
        battleActionType = BattleActionType.Default;
        takenAction = false;

    }

    #region ACTIONS

    public virtual void Defend()
    {
        if(takenAction)
        {
            return;
        }
        print(nameCharacter + " is defending");
        Defence += baseDefence / 2;
        battleActionType = BattleActionType.Defend;
        targets.Add(this);
        takenAction = true;
    }

    public void StopDefend()
    {
        animator.SetBool("Defending", false);
        print(nameCharacter + " has stopped defending");
        Defence = baseDefence;
    }
    
    private void Attack()
    {
        PlayAttackSFX();
        animator.SetTrigger("Attack");
        print(nameCharacter + " attacks " + targets[0].nameCharacter);
    }

    private void Ability()
    {
        animator.SetTrigger("Ability");
        // play anim and sfx
        foreach ( var target in targets)
        {
            print(nameCharacter + " cast " + activeAbility.abilityName + " at " + target.name);
        }
    }

    #endregion

    private void AttemptDamage(int damage)
    {
        TakeDamage(damage);
    }  

    // Called in animation event
    public void AttemptDamageCall()
    {
        animator.SetBool("TargetSelected", false);
        targets[0].AttemptDamage(Power);
        StartCoroutine(DelayedBattleEngineUpdate(2));
    }

    // Called in animation event
    public void CastAbility()
    {
        AbilityManager.instance.PlayEffect(targets[0].transform, activeAbility.abilityName);
        StartCoroutine( DelayAbilityCall(activeAbility.castTime));
        AP -= activeAbility.apCost;
        UpdateStats();
    }

    public void AttemptAbilityCall()
    {
        animator.SetBool("TargetSelected", false);
        var combinedStrength = activeAbility.strength + Wisdom;
        /*
        foreach (BattleCharacter target in targets)
        {
            target.CalculateRes(activeAbility.element, combinedStrength);
        }
        */
        for (int i =0; i< targets.Count; i++)
        {
            targets[i].CalculateRes(activeAbility.element, combinedStrength);
        }
        
        StartCoroutine(DelayedBattleEngineUpdate(2));
    }

    private IEnumerator DelayAbilityCall(float waitTime)
    {
        // wait for hurt anim
        yield return new WaitForSeconds(waitTime);
        AttemptAbilityCall();
    }

    private IEnumerator DelayedBattleEngineUpdate(float waitTime)
    {
        // wait for hurt anim
        yield return new WaitForSeconds(waitTime);
        _engine.UpdateBattleEngine();
    }

    private void PlayAttackSFX()
    {
        AudioManager.instance.PlaySFXFromClip(attackSounds[0]);
    }

    private void PlayHurtSFX()
    {
        AudioManager.instance.PlaySFXFromClip(hurtSounds[0]);
    }
}
