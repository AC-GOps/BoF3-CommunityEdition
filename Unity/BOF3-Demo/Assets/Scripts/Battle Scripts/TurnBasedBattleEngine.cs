﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;


public enum BattleState
{
    NotInBattle, SelectAction, SelectTarget, SelectAbility, SelectTargetAbility
}

public class TurnBasedBattleEngine : MonoBehaviour
{
    public static TurnBasedBattleEngine Instance;
    public BattleUI battleUI;
    public SimpleSpriteSheetAnimator hitAnimator;

    private List<PlayerBattleCharacter> playerBattleCharacters = new List<PlayerBattleCharacter>();
    private List<EnemyBattleCharacter> enemyBattleCharacters = new List<EnemyBattleCharacter>();
    public List<BattleCharacter> battleCharacters = new List<BattleCharacter>();

    public BattleCharacter activeCharacter;
    public List<BattleCharacter> selectedTargets = new List<BattleCharacter>();
    public int activeCharacterCount;
    public int selectedCharacterCount;

    public List<Animator> playerAnimators;
    public List<EnemyAnimationContolller> enemyAnimators = new List<EnemyAnimationContolller>();

    public List<Transform> playerBattleLocations = new List<Transform>();
    public List<Transform> enemyBattleLocations = new List<Transform>();

    public bool choosingTarget;

    public CameraFollowTarget cameraFollowTarget;
    public Transform battleCamLocation;

    public int battleTurnCount;
    private bool battleWon;
    public float battleSpeed;

    private void Awake()
    {
        Instance = this;
        battleUI.state = BattleState.NotInBattle;
    }

    private void Update()
    {
        SelectTarget();
        GetInput();
    }

    private void GetInput()
    {
        //GO BACK
        if (Input.GetKeyDown(KeyCode.Z))
        {
            switch(battleUI.state)
            {
                case BattleState.NotInBattle:
                    break;
                case BattleState.SelectAbility:
                    battleUI.ToggleAbilityMenu(false);
                    battleUI.ToggleBattleInfo(false);
                    battleUI.TogglePlayerBattleMenu(true);
                    battleUI.state=BattleState.SelectAction;
                    break;
                case BattleState.SelectAction:
                    MoveToPreviousCharacter();
                    break;
                case BattleState.SelectTarget:
                    battleUI.ToggleSelectTargetMenu(false);
                    battleUI.TogglePlayerBattleMenu(true);
                    battleUI.UpdateNameSkillText(activeCharacter.nameCharacter);
                    for (int i = 0; i < selectedTargets.Count; i++)
                    {
                        selectedTargets[i].GetComponentInChildren<SelectedSpriteFlash>().StopFlash();
                        battleUI.UpdateTargetedUIAll(selectedTargets[i], i, false);
                    }
                    choosingTarget = false;
                    battleUI.state = BattleState.SelectAction;
                    break;
                case BattleState.SelectTargetAbility:
                    battleUI.ToggleSelectTargetMenu(false);
                    battleUI.ToggleAbilityMenu(true, activeCharacter.battleAbilities, AbilityType.ATTACK,true);
                    battleUI.TurnOnHiddenHealth();
                    choosingTarget = false;
                    for (int i = 0; i < selectedTargets.Count; i++)
                    {
                        selectedTargets[i].GetComponentInChildren<SelectedSpriteFlash>().StopFlash();
                        battleUI.UpdateTargetedUIAll(selectedTargets[i], i, false);
                    }
                    battleUI.state = BattleState.SelectAbility;
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (battleUI.state == BattleState.SelectAbility)
            {
                CheckAbilityCost();
            }

        }
    }

    public void Init()
    {
        SetupBattle();
    }

    // ------------------------------------------------- BATTLE SETUP --------------------------------------------------------

    #region BATTLE SETUP

    void SetupBattle()
    {
        ResetAll();

        battleWon = false;

        AudioManager.instance.PlayMusic(1);

        playerBattleCharacters = new List<PlayerBattleCharacter>(PlayerCharacterManager.instance.playerBattleCharacters);
        enemyBattleCharacters = new List<EnemyBattleCharacter>(PlayerCharacterManager.instance.enemyBattleCharacters);

        foreach (var player in playerBattleCharacters)
        {
            playerAnimators.Add(player.GetComponentInChildren<Animator>());
            battleCharacters.Add(player);
            player.gameObject.SetActive(true);
        }

        foreach (var enemy in enemyBattleCharacters)
        {
            enemyAnimators.Add(enemy.GetComponentInParent<EnemyAnimationContolller>());
            battleCharacters.Add(enemy);
        }

        //Setup Camera
        cameraFollowTarget.target = battleCamLocation;

        //Setup players
        PlayerCharacterManager.instance.playerCharacterController.enabled = false;
        PlayerCharacterManager.instance.interactionManager.enabled = false;

        for(int i = 0; i < playerBattleCharacters.Count; i++)
        {
            playerBattleCharacters[i].transform.parent.position = playerBattleLocations[i].position;
            playerAnimators[i].SetTrigger("EnterBattle");
        }

        activeCharacterCount = 0;
        activeCharacter = playerBattleCharacters[activeCharacterCount];


        //Setup Enemies
        for (int i = 0; i < enemyBattleCharacters.Count; i++)
        {
            enemyBattleCharacters[i].transform.parent.position = enemyBattleLocations[i].position;
            enemyAnimators[i].SetAnimatorsTriggers();
            enemyBattleCharacters[i].SetupEnemyFirstTime();
        }

        // Do Health bars
        battleUI.InitEnemyHealthBars(enemyBattleCharacters);
        battleUI.InitPlayerHealthBars(playerBattleCharacters);

        // Determine Attack Order
        battleCharacters = battleCharacters.OrderByDescending(character => character.Agility).ToList();

        SetupTurn();
    }

    public void SetupAfterEnemyDeath(EnemyBattleCharacter deadCharacter)
    {
        battleTurnCount = -1;
        battleUI.UIOnDeath(deadCharacter);
        enemyBattleCharacters.Remove(deadCharacter);

        if (enemyBattleCharacters.Count == 0)
        {
            battleWon = true;
            return;
        }

        battleCharacters.Remove(deadCharacter);
        battleCharacters = battleCharacters.OrderByDescending(character => character.Agility).ToList();
        for (int i = 0; i < playerBattleCharacters.Count; i++)
        {
            var player = playerBattleCharacters[i];

            for (int j = 0; j < player.targets.Count; j++)
            {
                var target = player.targets[j];

                if (player.targets.Count > 1)
                {
                    player.targets.Remove(deadCharacter);
                    return;
                }

                if (target == deadCharacter)
                {
                    // single target
                    player.targets[0] = enemyBattleCharacters[0];
                }
            }
        }

    }

    public void SetupTurn()
    {
        print("Setup Turn ");
        foreach(var player in battleCharacters)
        {
            player.TurnStartReset();
        }
        activeCharacterCount = 0;
        activeCharacter = playerBattleCharacters[activeCharacterCount];
        battleCharacters = battleCharacters.OrderByDescending(character => character.Agility).ToList();
        // This is hard coded for the intro anims
        StartCoroutine(PlayerTurn(2));
    }

    private void ResetAll()
    {
        playerBattleCharacters.Clear();
        enemyBattleCharacters.Clear();
        battleCharacters.Clear();
        enemyAnimators.Clear();
        playerAnimators.Clear();
    }
    #endregion

    // ------------------------------------------------- PLAYER TURN --------------------------------------------------------

    #region PLAYER TURN
    private IEnumerator PlayerTurn(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        battleUI.TogglePlayerBattleMenu(true);
        battleUI.ToggleBattleHUD(true);
        battleUI.ToggleNameSkill(true);
        battleUI.UpdateNameSkillText(activeCharacter.nameCharacter);
        battleUI.state = BattleState.SelectAction;
    }

    public void onAttackButton()
    {
        AudioManager.instance.PlaySFX(SFX.Confirm);
        activeCharacter.battleActionType = BattleActionType.Attack;
        battleUI.UpdateNameSkillText(activeCharacter.battleActionType.ToString());
        battleUI.TogglePlayerBattleMenu(false);
        MoveToSelectTarget();
    }

    public void onDefendButton()
    {
        AudioManager.instance.PlaySFX(SFX.Confirm);
        activeCharacter.Defend();
        battleUI.ToggleNameSkill(false);
        MoveToNextCharacter();
    }

    public void onAbilityButton()
    {
        AudioManager.instance.PlaySFX(SFX.Confirm);
        // TODO Open Ability Menu
        battleUI.TogglePlayerBattleMenu(false);
        battleUI.ToggleAbilityMenu(true, activeCharacter.battleAbilities);
        battleUI.ToggleBattleInfo(true);
        StartCoroutine(DelayAction()); 
    }

    private IEnumerator DelayAction()
    {
        yield return new WaitForSeconds(0.2f);
        battleUI.state = BattleState.SelectAbility;
    }

    private void CheckAbilityCost()
    {
        var ability = battleUI.abilityMenu.currentUISelected.ability;
        if(activeCharacter.AP - ability.apCost < 0)
        {
            AudioManager.instance.PlaySFX(SFX.Error);
            print("Cannot afford abiltiy");
            return;
        }
        SetAbility();
    }

    public void SetAbility()
    {
        activeCharacter.battleActionType = BattleActionType.Ability;
        activeCharacter.activeAbility = battleUI.abilityMenu.currentUISelected.ability;
        battleUI.ToggleAbilityMenu(false);
        if(activeCharacter.activeAbility.targetAll)
        {
            MoveToSelectAll();
            return;
        }
        MoveToSelectTarget();
    }

    private void MoveToNextCharacter()
    {
        activeCharacterCount++;
        selectedTargets.Clear();

        if (activeCharacterCount > playerBattleCharacters.Count - 1)
        {
            // all charcter have put in thier input
            StartCombat();
            return;
        }

        activeCharacter = playerBattleCharacters[activeCharacterCount];
        StartCoroutine(PlayerTurn(battleSpeed));
    }

    private void MoveToPreviousCharacter()
    {
        battleUI.TogglePlayerBattleMenu(false);
        battleUI.ToggleNameSkill(false);
        activeCharacterCount--;
        if (activeCharacterCount < 0)
        {
            // back to start
            activeCharacterCount = 0;
        }
        activeCharacter = playerBattleCharacters[activeCharacterCount];
        activeCharacter.targets = null;
        activeCharacter.battleActionType = BattleActionType.Default;
        activeCharacter.TurnStartReset();
        StartCoroutine(PlayerTurn(battleSpeed));
    }

    #endregion

    #region SELECT TARGET

    private void SelectTarget()
    {
        if (!choosingTarget)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            selectedCharacterCount++;
            if (selectedCharacterCount > enemyBattleCharacters.Count - 1)
            {
                selectedCharacterCount = 0;
            }
            SetSelectedTarget();
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            selectedCharacterCount--;
            if (selectedCharacterCount < 0)
            {
                selectedCharacterCount = enemyBattleCharacters.Count - 1;
            }
            SetSelectedTarget();
        }

    }

    private void MoveToSelectTarget()
    {
        selectedCharacterCount = 0;
        selectedTargets.Clear();
        selectedTargets.Add(enemyBattleCharacters[selectedCharacterCount]);
        battleUI.ToggleSelectTargetMenu(true); 
        choosingTarget = true;

        battleUI.state = BattleState.SelectTarget;
        if (activeCharacter.battleActionType == BattleActionType.Ability)
        {
            battleUI.state = BattleState.SelectTargetAbility;
        }

        SetSelectedTarget();
    }

    private void MoveToSelectAll()
    {
        selectedTargets = enemyBattleCharacters.Cast<BattleCharacter>().ToList();
        //selectedTargets = enemyBattleCharacters;
        battleUI.ToggleSelectTargetMenu(true);

        battleUI.state = BattleState.SelectTarget;
        if (activeCharacter.battleActionType == BattleActionType.Ability)
        {
            battleUI.state = BattleState.SelectTargetAbility;
        }

        SetSelectedTargetAll();
    }

    private void SetSelectedTargetAll()
    {
        AudioManager.instance.PlaySFX(SFX.Select);

        for(int i = 0; i < selectedTargets.Count; i++)
        {
            selectedTargets[i].GetComponentInChildren<SelectedSpriteFlash>().StartFlash();
            battleUI.UpdateTargetedUIAll(selectedTargets[i], i, true);
        }
    }

    private void SetSelectedTarget()
    {
        AudioManager.instance.PlaySFX(SFX.Select);
        selectedTargets[0].GetComponentInChildren<SelectedSpriteFlash>().StopFlash();
        selectedTargets[0] = enemyBattleCharacters[selectedCharacterCount];
        selectedTargets[0].GetComponentInChildren<SelectedSpriteFlash>().StartFlash();
        battleUI.UpdateTargetedUI(selectedTargets[0]);
    }

    // Attached to button in scene
    public void OnSelectTarget()
    {
        AudioManager.instance.PlaySFX(SFX.Confirm);
        for (int i = 0; i < selectedTargets.Count; i++)
        {
            selectedTargets[i].GetComponentInChildren<SelectedSpriteFlash>().StopFlash();
            battleUI.UpdateTargetedUIAll(selectedTargets[i], i, false);
        }
        choosingTarget = false;
        activeCharacter.targets = new List<BattleCharacter>(selectedTargets);
        //activeCharacter.animator.SetTrigger(activeCharacter.battleActionType.ToString() + "BattleStance");
        activeCharacter.animator.SetTrigger("AttackBattleStance");
        activeCharacter.animator.SetBool("TargetSelected", true);
        battleUI.ToggleSelectTargetMenu(false);
        battleUI.TurnOnHiddenHealth();
        battleUI.ToggleNameSkill(false);
        battleUI.ToggleBattleInfo(false);
        MoveToNextCharacter();
    }

    // TODO change this to a genreic changeTarget and make new list of targetedCharacters which gets updated depending on enemy or player

    public void OnSelectAlly()
    {
        //selectedTargets.GetComponentInChildren<SelectedSpriteFlash>().StopFlash();
        choosingTarget = false;
        //activeCharacter.targets[0] = selectedTargets;
        activeCharacter.animator.SetTrigger(activeCharacter.battleActionType.ToString() + "BattleStance");
        activeCharacter.animator.SetBool("TargetSelected", true);
        MoveToNextCharacter();
    }
    #endregion

    // ------------------------------------------------- ENEMY AI --------------------------------------------------------

    public void EnemyAI()
    {
        foreach(EnemyBattleCharacter enemy in enemyBattleCharacters)
        {
            var random = Random.Range(0, 100);
            if(random>50)
            {
                print(enemy.nameCharacter + " decided to Attack");
                enemy.targets.Add(playerBattleCharacters[Random.Range(0, playerBattleCharacters.Count)]);
                enemy.battleActionType = BattleActionType.Attack;
            }
            else
            {
                print(enemy.nameCharacter + " decided to Defend");
                enemy.targets.Add(enemy);
                enemy.battleActionType = BattleActionType.Defend;
            }
        }
    }

    // ------------------------------------------------- PLAY BATTLE --------------------------------------------------------

    private void StartCombat()
    {
        battleUI.TogglePlayerBattleMenu(false);
        battleTurnCount = -1;
        EnemyAI();
        UpdateBattleEngine(1);
    }

    public void PlayBattle()
    {
        if(battleWon)
        {
            AudioManager.instance.PlayMusic(0);
            battleUI.EnableBattleWon();
            foreach (var player in playerBattleCharacters)
            {
                player.animator.SetBool("Defending", false);
                player.animator.SetTrigger("Battle Won");
                player.animator.SetBool("TargetSelected", false);
            }
            return;
        }

        if(battleTurnCount>battleCharacters.Count-1)
        {
            //end of combat
            SetupTurn();
            return;
        }

        activeCharacter = battleCharacters[battleTurnCount];
        battleCharacters[battleTurnCount].AttemptAction();
    }

    public void PlayHitAnimation()
    {
        hitAnimator.transform.position = activeCharacter.targets[0].transform.position;
        hitAnimator.PlaySpriteAnimation();
    }

    // ------------------------------------------------- END BATTLE --------------------------------------------------------
    // Won Button
    public void ShowExpPanel()
    {
        //show exp panel here
        ExitComabt();
    }

    public void ExitComabt()
    {
        PlayerCharacterManager.instance.UpdatePlayerInfo(playerBattleCharacters);
        cameraFollowTarget.target = PlayerCharacterManager.instance.playerCharacterController.transform;
        PlayerCharacterManager.instance.playerCharacterController.enabled = true;
        PlayerCharacterManager.instance.interactionManager.enabled = true;
        PlayerCharacterManager.instance.interactionManager.canInteract = true;
        battleUI.ExitBattle();
        foreach (var player in playerBattleCharacters)
        {
            player.gameObject.SetActive(false);
            if (player.nameCharacter == PlayerCharacterManager.instance.battleCharacter.nameCharacter)
            {
                player.gameObject.SetActive(true);
            }
        }

    }

    // ------------------------------------------------- EXTRA --------------------------------------------------------

    public void UpdateBattleEngine(float time = 0)
    {
        StartCoroutine(DelayedBattleEngineUpdate(time));
    }

    private IEnumerator DelayedBattleEngineUpdate(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        print("Battle Turn updated");
        battleTurnCount++;
        PlayBattle();
    }
}