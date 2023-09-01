using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;


public enum BattleState
{
    NotInBattle, SelectAction, SelectTarget, SelectAbility
}

public class TurnBasedBattleEngine : MonoBehaviour
{
    public static TurnBasedBattleEngine Instance;
    public BattleUI battleUI;
    public SimpleSpriteSheetAnimator hitAnimator;

    private List<BattleCharacter> playerBattleCharacters = new List<BattleCharacter>();
    private List<BattleCharacter> enemyBattleCharacters = new List<BattleCharacter>();
    public List<BattleCharacter> battleCharacters = new List<BattleCharacter>();

    public BattleCharacter activeCharacter;
    public BattleCharacter selectedTarget;
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
    }

    public void Init()
    {
        StartCoroutine(SetupBattle());
    }

    // ------------------------------------------------- BATTLE SETUP --------------------------------------------------------

    #region BATTLE SETUP

    IEnumerator SetupBattle()
    {
        ResetAll();

        battleWon = false;

        AudioManager.instance.PlayMusic(1);

        playerBattleCharacters = new List<BattleCharacter>(PlayerCharacterManager.instance.playerBattleCharacters);
        enemyBattleCharacters = new List<BattleCharacter>(PlayerCharacterManager.instance.enemyBattleCharacters);

        yield return new WaitForSeconds(0.2f);

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

        // wait for entrance anims
        SetupTurn();
    }

    public void SetupAfterEnemyDeath(BattleCharacter deadCharacter)
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
        foreach(var player in playerBattleCharacters)
        {
            if(player.target == deadCharacter)
            {
                player.target = enemyBattleCharacters[0];
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
        StartCoroutine(PlayerTurn(1));
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
        battleUI.TogglePlayerBattleMenu(activeCharacter.nameCharacter, true);
        battleUI.ToggleBattleHUD(true);
        battleUI.state = BattleState.SelectAction;
    }

    public void onAttackButton()
    {
        battleUI.ToggleSelectTargetMenu(true);
        AudioManager.instance.PlaySFX(SFX.Confirm);
        activeCharacter.battleActionType = BattleActionType.Attack;
        selectedCharacterCount = 0;
        selectedTarget = enemyBattleCharacters[selectedCharacterCount];
        MoveToSelectTarget();

    }

    public void onDefendButton()
    {
        AudioManager.instance.PlaySFX(SFX.Confirm);
        activeCharacter.Defend();
        MoveToNextCharacter();
    }

    public void onAbilityButton()
    {
        AudioManager.instance.PlaySFX(SFX.Confirm);

        activeCharacter.battleActionType = BattleActionType.Ability;

        // if player is target
        selectedCharacterCount = 0;
        selectedTarget = playerBattleCharacters[selectedCharacterCount];

        // TODO Open Ability Menu

    }

    private void MoveToNextCharacter()
    {
        activeCharacterCount++;

        if (activeCharacterCount > playerBattleCharacters.Count - 1)
        {
            // all charcter have put in thier input
            StartCombat();
            return;
        }

        activeCharacter = playerBattleCharacters[activeCharacterCount];
        StartCoroutine(PlayerTurn(0));
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

        activeCharacter = playerBattleCharacters[activeCharacterCount];
        StartCoroutine(PlayerTurn(0));
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
        battleUI.TogglePlayerBattleMenu("", false);
        choosingTarget = true;
        battleUI.state = BattleState.SelectTarget;
        SetSelectedTarget();
    }

    private void SetSelectedTarget()
    {
        AudioManager.instance.PlaySFX(SFX.Select);
        selectedTarget.GetComponentInChildren<SelectedSpriteFlash>().StopFlash();
        selectedTarget = enemyBattleCharacters[selectedCharacterCount];
        selectedTarget.GetComponentInChildren<SelectedSpriteFlash>().StartFlash();
        battleUI.UpdateTargetedUI(selectedTarget);
    }

    // Attached to button in scene
    public void OnSelectTarget()
    {
        AudioManager.instance.PlaySFX(SFX.Confirm);
        selectedTarget.GetComponentInChildren<SelectedSpriteFlash>().StopFlash();
        choosingTarget = false;
        activeCharacter.target = selectedTarget;
        activeCharacter.animator.SetTrigger(activeCharacter.battleActionType.ToString() + "BattleStance");
        activeCharacter.animator.SetBool("TargetSelected", true);
        battleUI.ToggleSelectTargetMenu(false);
        battleUI.TurnOnHiddenHealth();
        MoveToNextCharacter();
    }

    // TODO change this to a genreic changeTarget and make new list of targetedCharacters which gets updated depending on enemy or player

    public void OnSelectAlly()
    {
        selectedTarget.GetComponentInChildren<SelectedSpriteFlash>().StopFlash();
        choosingTarget = false;
        activeCharacter.target = selectedTarget;
        activeCharacter.animator.SetTrigger(activeCharacter.battleActionType.ToString() + "BattleStance");
        activeCharacter.animator.SetBool("TargetSelected", true);
        battleUI.selectTargetMenu.SetActive(false);
        MoveToNextCharacter();
    }
    #endregion

    // ------------------------------------------------- ENEMY AI --------------------------------------------------------

    public void EnemyAI()
    {
        foreach(BattleCharacter enemy in enemyBattleCharacters)
        {
            var random = Random.Range(0, 100);
            if(random>50)
            {
                print(enemy.nameCharacter + " decided to Attack");
                enemy.target = playerBattleCharacters[Random.Range(0, playerBattleCharacters.Count)];
                enemy.battleActionType = BattleActionType.Attack;
            }
            else
            {
                print(enemy.nameCharacter + " decided to Defend");
                enemy.target = enemy;
                enemy.battleActionType = BattleActionType.Defend;
            }
        }
    }

    // ------------------------------------------------- PLAY BATTLE --------------------------------------------------------

    private void StartCombat()
    {
        battleUI.TogglePlayerBattleMenu("", false);
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
        hitAnimator.transform.position = activeCharacter.target.transform.position;
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
