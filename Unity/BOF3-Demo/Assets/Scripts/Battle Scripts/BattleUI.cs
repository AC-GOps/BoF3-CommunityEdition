using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;
using DG.Tweening;

public class BattleUI : MonoBehaviour
{
    public static BattleUI instance { get; private set; }

    public GameObject playerbattlemenu;
    public GameObject selectTargetMenu;
    public GameObject battleNameSKillPanel;
    public GameObject battleInfoPanel;
    public GameObject playerbattleHUD;
    public GameObject playerWonPanel;
    public GameObject attackButton, defendButton, observeButton, itemButton, abilityButton;
    public GameObject wonButton;
    public TMP_Text nameSkillText;
    public TMP_Text battleInfoText;
    public EventSystem eventSystem;

    public List<EnemyHealthBarUI> EnemyHealthBars = new List<EnemyHealthBarUI>();
    private int EnemyTypeAmount;
    public GameObject EnemyHealthBarsParent;

    public UIToEnemy uiToEnemy;

    public List<PlayerHealthBarUI> PlayerHealthBars = new List<PlayerHealthBarUI>();
    public GameObject currentHiddenHealth;

    public BattleState state;
    private bool hasInput;

    private bool ShowInfo;
    private float timer;
    public float showInfoTime;

    private void Awake()
    {
        instance = this;

    }

    private void Update()
    {
        BattleUIInput();
        ShowInfoTimer();
    }

    private void BattleUIInput()
    {
        if(state == BattleState.NotInBattle)
        {
            return;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (state == BattleState.SelectAction)
        {
            if (input == Vector2.zero)
            {
                eventSystem.SetSelectedGameObject(attackButton);
                hasInput = false;
                return;
            }

            if (hasInput)
            {
                return;
            }

            AudioManager.instance.PlaySFX(SFX.Select);
            hasInput = true;
            if (input.x != 0)
            {
                eventSystem.SetSelectedGameObject(input.x > 0 ? defendButton : observeButton);
                return;
            }
            eventSystem.SetSelectedGameObject(input.y > 0 ? abilityButton : itemButton);
        }
    }

    public void InitEnemyHealthBars(List<EnemyBattleCharacter> enemies)
    {
        EnemyTypeAmount = 0;
        EnemyHealthBarsParent.GetComponentsInChildren<EnemyHealthBarUI>(true, EnemyHealthBars);

        foreach (EnemyHealthBarUI healthBarUI in EnemyHealthBars)
        {
            healthBarUI.healthBarName.text = "";
            healthBarUI.owner.Clear();
            healthBarUI.enemyAmount = 0;
        }

        foreach (EnemyBattleCharacter enemy in enemies)
        {
            var v = EnemyHealthBars.Find(i => i.healthBarName.text == enemy.nameCharacter);

            if(v != null)
            {
                v.enemyAmount++;
                enemy.ammount = v.enemyAmount;
                v.healthBarsRed[v.enemyAmount].gameObject.SetActive(true);
                v.owner.Add(enemy);
                enemy.healthBarUI = v;
            }
            else
            {
                enemy.ammount = 0;
                var EHB = EnemyHealthBars[EnemyTypeAmount];
                EHB.gameObject.SetActive(true);
                EHB.healthBarsRed[0].gameObject.SetActive(true);
                EHB.owner.Add(enemy);
                enemy.healthBarUI = EHB;
                EHB.healthBarName.text = enemy.nameCharacter;
                EnemyTypeAmount++;
            }
            enemy.UpdateHealth(true);
        }
    }

    public void InitPlayerHealthBars(List<PlayerBattleCharacter> players)
    {

        foreach (PlayerHealthBarUI healthBarUI in PlayerHealthBars)
        {
            healthBarUI.healthBarName.text = "";
            healthBarUI.gameObject.SetActive(false);
        }

        for(int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            players[i].playerHealthBar = PlayerHealthBars[i];

            var PHB = players[i].playerHealthBar;
            PHB.gameObject.SetActive(true);
            PHB.healthBarRed.gameObject.SetActive(true);
            //player.healthbar = PHB.healthBar;
            //player.healthbarRed = PHB.healthBarRed;
            player.UpdateHealth(true);
        }
    }

    public void UpdateTargetedUI(EnemyBattleCharacter selectedTarget)
    {
        uiToEnemy.ChangePosition(selectedTarget.transform.position);
        uiToEnemy.SetEnemyInfo(selectedTarget);

        var v = EnemyHealthBars.Find(i => i.healthBarName.text == selectedTarget.nameCharacter);
        var t = v.owner.IndexOf(selectedTarget);
        TurnOffHealthBar(t, v);
    }

    private void TurnOffHealthBar(int index, EnemyHealthBarUI enemyHealth)
    {
        
        if (currentHiddenHealth != null)
        {
            currentHiddenHealth.SetActive(true);
        }
        

        if (enemyHealth.owner.Count > 1)
        {
            //print("more than 1 owner");
            currentHiddenHealth = enemyHealth.healthBarsRed[index].gameObject;
            currentHiddenHealth.SetActive(false);
            return;
        }

        //print("only 1 owner");
        currentHiddenHealth = EnemyHealthBars[index].gameObject;
        currentHiddenHealth.SetActive(false);
    }

    private void KillHealthBar(int index, EnemyHealthBarUI enemyHealth)
    {
        if (enemyHealth.owner.Count > 1)
        {
            enemyHealth.healthBarsRed[index].gameObject.SetActive(false);
        }
    }

    public void UIOnDeath(EnemyBattleCharacter character)
    {
        var v = EnemyHealthBars.Find(i => i.healthBarName.text == character.nameCharacter);
        //print("Health bar name that just died is " + v.name);
        var t = v.owner.IndexOf(character);
        KillHealthBar(t, v);
        RemoveOwnerFromList(character, v);
    }

    public void TurnOnHiddenHealth()
    {
        //print("setting health active");
        currentHiddenHealth.SetActive(true);
        currentHiddenHealth = null;
    }

    public void ToggleBattleInfo(bool on)
    {
        battleInfoPanel.transform.DOScale(on ? 1 : 0, 0.5f);
    }

    public void UpdateBattleInfo(string text)
    {
        battleInfoText.text = text;
    }

    public void PulseBattleInfo(string text)
    {
        UpdateBattleInfo(text);
        if(!ShowInfo)
        {
            ToggleBattleInfo(true);
        }
        ShowInfo = true;
        timer = 0;
    }

    private void ShowInfoTimer()
    {
        if(!ShowInfo)
        { return; }
        timer += Time.deltaTime;
        if(timer > showInfoTime)
        {
            ShowInfo = false;
            ToggleBattleInfo(false);
        }
    }

    private void RemoveOwnerFromList(EnemyBattleCharacter owner, EnemyHealthBarUI v)
    {
        v.owner.Remove(owner);
        if (v.owner.Count > 0)
        {
            return;
        }
        v.gameObject.SetActive(false);
        EnemyHealthBars.Remove(v);
    }

    public void ToggleBattleHUD(bool active)
    {
        playerbattleHUD.SetActive(active);
    }

    public void ToggleSelectTargetMenu(bool active)
    {
        selectTargetMenu.SetActive(active);
        if (!active)
        {
            return;
        }
        eventSystem.SetSelectedGameObject(selectTargetMenu);
    }

    public void TogglePlayerBattleMenu(bool active)
    {
        playerbattlemenu.SetActive(active);
        if(!active)
        {
            return;
        }
        eventSystem.SetSelectedGameObject(attackButton);
    }

    public void ToggleNameSkill(bool active)
    {
        battleNameSKillPanel.SetActive(active);
    }

    public void UpdateNameSkillText(string text)
    {
        nameSkillText.text = text;
    }
    
    public void EnableBattleWon()
    {
        playerbattlemenu.SetActive(false);
        playerbattleHUD.SetActive(false);
        playerWonPanel.SetActive(true);
        eventSystem.SetSelectedGameObject(wonButton);
    }

    public void ExitBattle()
    {
        playerWonPanel.SetActive(false);
    }
}
