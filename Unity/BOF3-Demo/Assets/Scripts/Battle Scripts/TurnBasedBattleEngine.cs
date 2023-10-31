using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.EventSystems;
using DG.Tweening;


public enum BattleState
{
    NotInBattle, SelectAction, SelectTarget, SelectAbility, SelectTargetAbility, Battle
}

public class TurnBasedBattleEngine : MonoBehaviour
{
    public static TurnBasedBattleEngine Instance;
    public BattleUI battleUI;
    public SimpleSpriteSheetAnimator hitAnimator;
    public RandomBattleManager randomBattleManager;

    public List<PlayerBattleCharacter> playerBattleCharacters = new List<PlayerBattleCharacter>();
    private List<PlayerBattleCharacter> savedPlayerBattleCharacters = new List<PlayerBattleCharacter>();
    public List<EnemyBattleCharacter> enemyBattleCharacters = new List<EnemyBattleCharacter>();
    public List<BattleCharacter> battleCharacters = new List<BattleCharacter>();

    public BattleCharacter activeCharacter;
    public List<BattleCharacter> selectedTargets = new List<BattleCharacter>();
    public int activeCharacterCount;
    public int selectedCharacterCount;

    public List<Animator> playerAnimators;
    public List<EnemyAnimationContolller> enemyAnimators = new List<EnemyAnimationContolller>();

    public List<Transform> playerBattleLocations = new List<Transform>();
    public List<Transform> enemyBattleLocations = new List<Transform>();

    public CameraFollowTarget cameraFollowTarget;
    public Transform battleCamLocation;

    public int battleTurnCount;
    private bool battleWon;
    private bool battleLost;
    public float battleSpeed;

    private void Awake()
    {
        Instance = this;
        battleUI.state = BattleState.NotInBattle;
    }

    public void BattleBack(InputAction.CallbackContext context)
    {
        if (battleUI.state == BattleState.Battle)
        {
            return;
        }

        //GO BACK
        if (context.performed)
        {
            switch (battleUI.state)
            {
                case BattleState.NotInBattle:
                    break;
                case BattleState.SelectAbility:
                    battleUI.ToggleAbilityMenu(false);
                    battleUI.ToggleBattleInfo(false);
                    battleUI.TogglePlayerBattleMenu(true);
                    battleUI.state = BattleState.SelectAction;
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
                        battleUI.UpdateTargetedUI(selectedTargets[i]);
                    }
                    battleUI.TurnOnHiddenHealth();
                    battleUI.state = BattleState.SelectAction;
                    break;
                case BattleState.SelectTargetAbility:
                    battleUI.ToggleSelectTargetMenu(false);
                    battleUI.ToggleAbilityMenu(true, activeCharacter.battleAbilities, AbilityType.ATTACK, true);
                    activeCharacter.activeAbility = null;
                    for (int i = 0; i < selectedTargets.Count; i++)
                    {
                        selectedTargets[i].GetComponentInChildren<SelectedSpriteFlash>().StopFlash();
                        battleUI.UpdateTargetedUIAll(selectedTargets[i], i, false);
                    }
                    battleUI.TurnOnHiddenHealth();
                    battleUI.state = BattleState.SelectAbility;
                    break;
            }
        }
    }

    public void Init()
    {
        SetupBattle();
    }

    // ------------------------------------------------- BATTLE SETUP --------------------------------------------------------

    #region BATTLE SETUP

    public void SetupBattleLocations(List<Transform> BattleLocations)
    {
        playerBattleLocations.Clear();
        enemyBattleLocations.Clear();
        for(int i = 0; i < BattleLocations.Count; i++)
        {
            if(i >= 1 && i <4)
            {
                playerBattleLocations.Add(BattleLocations[i]);
            }
            else if(i >= 4 && i < 7)
            {
                enemyBattleLocations.Add(BattleLocations[i]);
            }
            if (i == 7)
            {
                battleCamLocation = BattleLocations[i];
            }
        }
    }

    void SetupBattle()
    {
        ResetAll();

        PlayerInputManager.Instance.SwapActionMaps("Battle");

        battleWon = false;

        AudioManager.instance.PlayMusic(2);

        playerBattleCharacters = new List<PlayerBattleCharacter>(PlayerCharacterManager.instance.playerBattleCharacters);
        savedPlayerBattleCharacters = new List<PlayerBattleCharacter>(PlayerCharacterManager.instance.playerBattleCharacters);
        enemyBattleCharacters = PlayerCharacterManager.instance.CreateNewEnemyList();


        for (int i = 0; i < playerBattleCharacters.Count; i++)
        {
            var player = playerBattleCharacters[i];
            playerAnimators.Add(player.GetComponentInChildren<Animator>());
        }

        foreach (var player in playerBattleCharacters)
        {
            player.GetComponent<SpriteRenderer>().flipX = false;
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

        
        for (int i = 0; i < savedPlayerBattleCharacters.Count; i++)
        {
            var player = playerBattleCharacters[i];
            var render = player.GetComponent<SpriteRenderer>();
            /*
            if (player.isDead)
            {
                playerBattleCharacters.Remove(player);
                battleCharacters.Remove(player);
                playerAnimators[i].SetBool("Die", true);
            }
            else 
            {

            }
            */
            render.DOFade(0, 0f);
            player.transform.parent.position = playerBattleLocations[i].position;
            render.DOFade(1, 0.3f);
            playerAnimators[i].SetTrigger("EnterBattle");
            playerAnimators[i].speed = 1;
        }


        activeCharacterCount = 0;
        activeCharacter = playerBattleCharacters[activeCharacterCount];


        //Setup Enemies
        for (int i = 0; i < enemyBattleCharacters.Count; i++)
        {
            var render = enemyBattleCharacters[i].GetComponent<SpriteRenderer>();
            render.DOFade(0, 0f);
            enemyBattleCharacters[i].transform.parent.position = enemyBattleLocations[i].position;
            render.DOFade(1, 0.3f);
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
        enemyBattleCharacters.Remove(deadCharacter);
        battleUI.EnemyUIOnDeath(deadCharacter);
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
            if(player == activeCharacter)
            {
                return;
            }

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

    public void SetupAfterPlayerDeath(PlayerBattleCharacter deadCharacter)
    {
        battleTurnCount = -1;
        playerBattleCharacters.Remove(deadCharacter);
        if (playerBattleCharacters.Count == 0)
        {
            battleLost = true;
            return;
        }

        battleCharacters.Remove(deadCharacter);
        battleCharacters = battleCharacters.OrderByDescending(character => character.Agility).ToList();
        for (int i = 0; i < enemyBattleCharacters.Count; i++)
        {
            var enemy = enemyBattleCharacters[i];

            for (int j = 0; j < enemy.targets.Count; j++)
            {
                var target = enemy.targets[j];

                if (enemy.targets.Count > 1)
                {
                    enemy.targets.Remove(deadCharacter);
                    return;
                }

                if (target == deadCharacter)
                {
                    // single target
                    enemy.targets[0] = playerBattleCharacters[0];
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

    public void CheckAbilityCost()
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
        battleUI.TogglePlayerBattleMenu(false);
        battleUI.ToggleNameSkill(false);
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
        activeCharacterCount--;
        if (activeCharacterCount < 0)
        {
            // back to start
            activeCharacterCount = 0;
            return;
        }
        battleUI.TogglePlayerBattleMenu(false);
        battleUI.ToggleNameSkill(false);
        activeCharacter = playerBattleCharacters[activeCharacterCount];
        activeCharacter.TurnStartReset();
        StartCoroutine(PlayerTurn(battleSpeed));
    }

    #endregion

    #region SELECT TARGET

    private void MoveToSelectTarget()
    {
        selectedCharacterCount = 0;
        selectedTargets.Clear();
        selectedTargets.Add(enemyBattleCharacters[selectedCharacterCount]);
        battleUI.ToggleSelectTargetMenu(true); 
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

    public void SetSelectedTarget()
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
            battleUI.TurnOnHiddenHealth();
        }

        activeCharacter.targets = new List<BattleCharacter>(selectedTargets);
        //activeCharacter.animator.SetTrigger(activeCharacter.battleActionType.ToString() + "BattleStance");
        activeCharacter.animator.SetTrigger("AttackBattleStance");
        activeCharacter.animator.SetBool("TargetSelected", true);
        battleUI.ToggleSelectTargetMenu(false);
        battleUI.TurnOnHiddenHealth();
        battleUI.ToggleNameSkill(false);
        battleUI.ToggleBattleInfo(false);
        foreach(BattleCharacter b in activeCharacter.targets)
        {
            print(activeCharacter.nameCharacter + " has targeted " + b.nameCharacter);
        }

        MoveToNextCharacter();
    }

    // TODO change this to a genreic changeTarget and make new list of targetedCharacters which gets updated depending on enemy or player

    public void OnSelectAlly()
    {
        //selectedTargets.GetComponentInChildren<SelectedSpriteFlash>().StopFlash();
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
            if(enemy.battleAbilities.Count>0)
            {
                if (enemy.AP >= enemy.battleAbilities[0].apCost)
                {

                    enemy.activeAbility = enemy.battleAbilities[0];
                    enemy.battleActionType = BattleActionType.Ability;
                    print(enemy.nameCharacter + " decided to cast " + enemy.activeAbility);
                    if (enemy.activeAbility.targetAll)
                    {
                        enemy.targets.AddRange(playerBattleCharacters);
                    }
                    else
                    {
                        enemy.targets.Add(playerBattleCharacters[Random.Range(0, playerBattleCharacters.Count)]);
                    }
                }
                else
                {
                    print("Enemy can't afford Ability");
                }
            }
            else
            {
                print(enemy.nameCharacter + "has no abilites");
            }

            if(enemy.battleActionType == BattleActionType.Default)
            {
                var random = Random.Range(0, 100);
                if (random < 70)
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
        battleUI.state = BattleState.Battle;
        if(battleWon)
        {
            AudioManager.instance.PlayMusic(1);
            battleUI.EnableBattleWon();
            foreach (var player in playerBattleCharacters)
            {
                player.animator.SetBool("Defending", false);
                player.animator.SetTrigger("Battle Won");
                player.animator.SetBool("TargetSelected", false);
            }
            return;
        }

        if(battleLost)
        {
            //AudioManager.instance.PlayMusic(0);
            battleUI.EnableGameOver();
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
        randomBattleManager.ResetRandomBattle();
        PlayerCharacterManager.instance.UpdatePlayerInfo(savedPlayerBattleCharacters);
        cameraFollowTarget.target = PlayerCharacterManager.instance.playerCharacterController.transform;
        PlayerCharacterManager.instance.playerCharacterController.enabled = true;
        PlayerCharacterManager.instance.interactionManager.enabled = true;
        PlayerCharacterManager.instance.interactionManager.canInteract = true;
        battleUI.ExitBattle();
        AreaManager.instance.ShowGameObjects();
        PlayerInputManager.Instance.SwapActionMaps("Gameplay");
        foreach (var player in PlayerCharacterManager.instance.playerBattleCharacters)
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
