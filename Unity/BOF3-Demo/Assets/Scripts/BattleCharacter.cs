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

public enum AbilityType
{
    Fire,
    Ice,
    Electric,
    Earth,
    Wind,
    Psyonic,
    Status,
    Death
}

public class BattleResitances
{
    public AbilityType abilityType;
    public int Level;
}

public class BattleCharacter : MonoBehaviour
{
    private TurnBasedBattleEngine _engine;
    public Animator animator;
    public BattleCharacter target;
    public BattleActionType battleActionType;
    public string nameCharacter;
    public int HP, maxHP;
    public int Defence, baseDefence;
    public int AP;
    public int Agility;
    public int Level;
    public int Dodge;
    public int Drop;
    public int DeathStat;
    public int Power;
    public int PsionicStatChance;
    public int Reprisal;
    public List<BattleResitances> Resistances; // Res against -100% - 300% Flame, Frost, Electric, Earth & Wind / Psionic, Status, Death -200% - 200%
    public int Wisdom;

    public List<AudioClip> attackSounds;
    public List<AudioClip> hurtSounds;

    public Image healthbar;
    public Image healthbarRed;

    public bool isDead;
    public bool takenAction;

    public PlayerHealthBarUI playerHealthBar;

    public void CalculateRes(AbilityType type, int damage)
    {
        var res = Resistances.Find(i => i.abilityType == type);
        if (res == null)
        {
            // no resistance found, play as if level 2 = 100%
            return;
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
    }

    private void Start()
    {
        Init();
        SetupEnemyFirstTime();
    }

    public void Init()
    {
        animator = GetComponentInChildren<Animator>();
        _engine = TurnBasedBattleEngine.Instance;
    }

    public void SetupEnemyFirstTime()
    {
        HP = maxHP;
        Defence = baseDefence;
    }

    private void DodgeAttack(int damage)
    {
        var chance = Random.Range(0, 100);
        if (chance < Dodge)
        {
            print(nameCharacter + " dodged attack");
            animator.SetTrigger("Dodge");
            damage = 0;
            StartCoroutine(DelayedBattleEngineUpdate(1));
            NumberBouncer.Instance.PlayTextBounceAtTarget(transform, "Miss");
        }

        TakeDamage(damage);
    }

    private void TakeDamage(int damage)
    {
        int actualDamage = damage - Defence;

        if (actualDamage < 0)
        {
            actualDamage = 0;
            print(nameCharacter + " took no damage");
            animator.SetTrigger("No Damage");
            StartCoroutine(DelayedBattleEngineUpdate(1));
            NumberBouncer.Instance.PlayNumberBounceAtTarget(transform, actualDamage);
            return;
        }

        PlayHurtSFX();

        HP -= actualDamage;
        print(nameCharacter + " took " + actualDamage + " damage");
        UpdateHealth();
        animator.SetTrigger("Hurt");
        TurnBasedBattleEngine.Instance.PlayHitAnimation();
        NumberBouncer.Instance.PlayNumberBounceAtTarget(transform, actualDamage);

        if (HP <= 0)
        {
            Die();
        }

        StartCoroutine(DelayedBattleEngineUpdate(2));
    }

    private void Die()
    {
        isDead = true;
        animator.SetBool("Die", true);
        print(nameCharacter + " died");
        _engine.SetupAfterEnemyDeath(this);
    }

    public void UpdateHealth(bool noFade = false)
    {
        if(playerHealthBar != null)
        {
            playerHealthBar.UpdateHealth(HP, maxHP, noFade);
            playerHealthBar.UpdateTexts(HP, AP, nameCharacter);
        }

    }

    public void AttemptAction()
    {
        if(takenAction)
        {
            print(nameCharacter + " has already taken action");
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
                print(nameCharacter + " is defending");
                StartCoroutine(DelayedBattleEngineUpdate(0.5f));
                break;
            case BattleActionType.Ability:
                animator.SetTrigger("Ability");
                print(nameCharacter + " attempts to attack " + target.nameCharacter);
                break;
            default:
                break;
        }

        takenAction = true;
    }

    public void TurnStartReset()
    {
        animator.SetBool("TargetSelected", false);

        if (battleActionType == BattleActionType.Defend)
        {
            StopDefend();
        }

        target = null;
        battleActionType = BattleActionType.Default;
        takenAction = false;

    }

    #region ACTIONS

    public void Defend()
    {
        if(takenAction)
        {
            return;
        }

        print(nameCharacter + " will defend themeselves");
        Defence += baseDefence / 2;
        battleActionType = BattleActionType.Defend;
        target = this;
        animator.SetBool("Defending", true);
    }

    public void StopDefend()
    {
        animator.SetBool("Defending", false);
        print(nameCharacter + " has stopped defending");
        Defence -= baseDefence / 2;
    }
    
    private void Attack()
    {
        PlayAttackSFX();
        animator.SetTrigger("Attack");
        print(nameCharacter + " attacks " + target.nameCharacter);
    }

    private void Ability()
    {
        // do ability here
    }

    #endregion

    private void AttemptDamage(int damage)
    {
        DodgeAttack(damage);
    }  

    public void AttemptDamageCall()
    {
        animator.SetBool("TargetSelected", false);
        target.AttemptDamage(Power);
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
