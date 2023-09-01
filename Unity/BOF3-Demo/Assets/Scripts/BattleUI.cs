using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;

public class BattleUI : MonoBehaviour
{
    public GameObject playerbattlemenu;
    public GameObject selectTargetMenu;
    public GameObject playerbattleHUD;
    public GameObject playerWonPanel;
    public GameObject attackButton, defendButton, observeButton, itemButton, abilityButton;
    public GameObject wonButton;
    public TMP_Text infoText;
    public EventSystem eventSystem;

    public List<EnemyHealthBarUI> EnemyHealthBars = new List<EnemyHealthBarUI>();
    private int EnemyTypeAmount;
    public GameObject EnemyHealthBarsParent;

    public UIToEnemy uiToEnemy;

    public List<PlayerHealthBarUI> PlayerHealthBars = new List<PlayerHealthBarUI>();
    public GameObject currentHiddenHealth;

    public BattleState state;
    private bool hasInput;

    private void Update()
    {
        BattleUIInput();
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

    public void InitEnemyHealthBars(List<BattleCharacter> enemies)
    {
        EnemyTypeAmount = 0;
        EnemyHealthBarsParent.GetComponentsInChildren<EnemyHealthBarUI>(true, EnemyHealthBars);

        foreach (EnemyHealthBarUI healthBarUI in EnemyHealthBars)
        {
            healthBarUI.healthBarName.text = "";
            healthBarUI.owner.Clear();
            healthBarUI.enemyAmount = 0;
        }

        foreach (BattleCharacter enemy in enemies)
        {
            var v = EnemyHealthBars.Find(i => i.healthBarName.text == enemy.nameCharacter);

            print(enemy.name);

            if(v != null)
            {
                //print("added to exsiting");
                v.enemyAmount++;
                v.healthBarsRed[v.enemyAmount].gameObject.SetActive(true);
                v.owner.Add(enemy);
                enemy.healthbar = v.healthBars[v.enemyAmount];
                enemy.healthbarRed = v.healthBarsRed[v.enemyAmount];
            }
            else
            {
                //print("made new list");
                var EHB = EnemyHealthBars[EnemyTypeAmount];
                EHB.gameObject.SetActive(true);
                EHB.healthBarsRed[0].gameObject.SetActive(true);
                EHB.owner.Add(enemy);
                enemy.healthbar = EHB.healthBars[0];
                enemy.healthbarRed = EHB.healthBarsRed[0];
                EHB.healthBarName.text = enemy.nameCharacter;
                EnemyTypeAmount++;
            }
            enemy.UpdateHealth(true);
        }
    }

    public void InitPlayerHealthBars(List<BattleCharacter> players)
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
            player.healthbar = PHB.healthBar;
            player.healthbarRed = PHB.healthBarRed;
            player.UpdateHealth(true);
        }
    }

    public void UpdateTargetedUI(BattleCharacter selectedTarget)
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

    public void UIOnDeath(BattleCharacter character)
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

    private void RemoveOwnerFromList(BattleCharacter owner, EnemyHealthBarUI v)
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

    public void TogglePlayerBattleMenu(string _infoText, bool active)
    {
        infoText.text = _infoText;
        playerbattlemenu.SetActive(active);
        if(!active)
        {
            return;
        }
        eventSystem.SetSelectedGameObject(attackButton);
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
