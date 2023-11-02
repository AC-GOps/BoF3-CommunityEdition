using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;
using DG.Tweening;

public class BattleUI : MonoBehaviour
{
    public static BattleUI instance { get; private set; }

    public AbilityMenu abilityMenu;
    public GameObject selectAbilityMenu;
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
    private int EnemyAmount;
    public GameObject EnemyHealthBarsParent;

    public UIToEnemy uiToEnemy;
    public List<UIToEnemy> uiToEnemys;

    public List<PlayerHealthBarUI> PlayerHealthBars = new List<PlayerHealthBarUI>();
    public List<NumberBouncer> NumberBouncers = new List<NumberBouncer>();
    public GameObject currentHiddenHealth;

    public BattleState state;
    private bool hasInput;

    private bool ShowInfo;
    private float timer;
    public float showInfoTime;

    public FadeImage gameoverfade;
    public GameObject playerGameOverPanel;
    public GameObject ContinueButton;

    private TurnBasedBattleEngine _engine;

    private VerticalLayoutGroup _verticalLayoutGroup;
    private CanvasGroup enemyhealthbarsGroup;

    private bool firstSetup;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _engine = TurnBasedBattleEngine.Instance;
    }

    private void Update()
    {
        ShowInfoTimer();
    }

    public void BattleSelectionInput(InputAction.CallbackContext context)
    {
        if(state == BattleState.NotInBattle)
        {
            return;
        }

        Vector2 input =context.ReadValue<Vector2>();

        if (input == Vector2.zero)
        {
            hasInput = false;
        }

        if (hasInput)
        {
            return;
        }

        if (state == BattleState.SelectAction)
        {
            if (input == Vector2.zero)
            {
                eventSystem.SetSelectedGameObject(attackButton);
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

        if (state == BattleState.SelectTarget || state == BattleState.SelectTargetAbility)
        {
            if(_engine.activeCharacter.activeAbility != null)
            {
                if(_engine.activeCharacter.activeAbility.targetAll)
                {
                    return;
                }
            }

            if(context.performed)
            {
                if (input == Vector2.zero)
                {
                    return;
                }

                hasInput = true;
                if (input.x > 0)
                {
                    _engine.selectedCharacterCount++;
                    if (_engine.selectedCharacterCount > _engine.enemyBattleCharacters.Count - 1)
                    {
                        _engine.selectedCharacterCount = 0;
                    }
                }
                if (input.x < 0)
                {
                    _engine.selectedCharacterCount--;
                    if (_engine.selectedCharacterCount < 0)
                    {
                        _engine.selectedCharacterCount = _engine.enemyBattleCharacters.Count - 1;
                    }
                }
                _engine.SetSelectedTarget();
            }
 
        }
    }

    public void BattleSelect(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if (state == BattleState.SelectAbility)
            {
                _engine.CheckAbilityCost();
            }
        }
    }

    public void InitEnemyHealthBars(List<EnemyBattleCharacter> enemies)
    {
        firstSetup = true;
        EnemyTypeAmount = 0;
        EnemyAmount = 0;
        EnemyHealthBarsParent.GetComponentsInChildren<EnemyHealthBarUI>(true, EnemyHealthBars);

        _verticalLayoutGroup = EnemyHealthBarsParent.GetComponent<VerticalLayoutGroup>();
        _verticalLayoutGroup.enabled = true;

        enemyhealthbarsGroup = EnemyHealthBarsParent.GetComponent<CanvasGroup>();
        enemyhealthbarsGroup.alpha = 0;

        foreach (EnemyHealthBarUI healthBarUI in EnemyHealthBars)
        {
            healthBarUI.healthBarName.text = "";
            healthBarUI.owner.Clear();
            healthBarUI.enemyAmount = 0;
            healthBarUI.gameObject.SetActive(false);
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
            enemy.UpdateStats(true);
            enemy.numBouncer = NumberBouncers[EnemyAmount];
            EnemyAmount++;
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
            PHB.owner = player;
            PHB.gameObject.SetActive(true);
            PHB.healthBarRed.gameObject.SetActive(true);
            player.UpdateStats(true);
        }
    }

    public void UpdateTargetedUI(BattleCharacter selectedTarget, bool tunOn = false)
    {
        uiToEnemy.ChangePosition(selectedTarget.transform.position);
        uiToEnemy.SetEnemyInfo(selectedTarget);

        var v = EnemyHealthBars.Find(i => i.healthBarName.text == selectedTarget.nameCharacter);
        var t = v.owner.IndexOf(selectedTarget);
        TurnOffHealthBar(t, v);
    }

    public void UpdateTargetedUIAll(BattleCharacter selectedTarget, int index, bool active)
    {
        uiToEnemys[index].gameObject.SetActive(active);
        uiToEnemys[index].ChangePosition(selectedTarget.transform.position);
        uiToEnemys[index].SetEnemyInfo(selectedTarget);
        if (index >= EnemyTypeAmount)
        {
            return;
        }
        EnemyHealthBars[index].gameObject.SetActive(!active);
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
        currentHiddenHealth = enemyHealth.gameObject;
        currentHiddenHealth.SetActive(false);
    }

    private void KillHealthBar(int index, EnemyHealthBarUI enemyHealth)
    {
        if (enemyHealth.owner.Count > 1)
        {
            enemyHealth.healthBarsRed[index].gameObject.SetActive(false);
            return;
        }
        foreach(Image ui in enemyHealth.healthBarsRed)
        {
            ui.gameObject.SetActive(false);
        }
        enemyHealth.gameObject.SetActive(false);
    }

    public void EnemyUIOnDeath(EnemyBattleCharacter character)
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
        if(currentHiddenHealth == null)
        {
            return;
        }
        currentHiddenHealth.SetActive(true);
        currentHiddenHealth = null;
    }

    #region BATTLE INFO
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
    #endregion
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
        if(!firstSetup)
        {
            return;
        }
        playerbattleHUD.GetComponent<CanvasGroup>().alpha = 0;
        StartCoroutine(ToggleVerticalLayout());
    }

    private IEnumerator ToggleVerticalLayout()
    {
        firstSetup = false;
        _verticalLayoutGroup.enabled = true;
        yield return new WaitForSeconds(0.1f);
        _verticalLayoutGroup.enabled = false;
        enemyhealthbarsGroup.alpha = 1;
        playerbattleHUD.GetComponent<CanvasGroup>().alpha = 1;
    }

    public void ToggleSelectTargetMenu(bool active)
    {
        eventSystem.SetSelectedGameObject(null);
        selectTargetMenu.SetActive(active);
        foreach(UIToEnemy ui in uiToEnemys)
        {
            ui.gameObject.SetActive(false);
        }
        uiToEnemy.gameObject.SetActive(active);
        if (!active)
        {
            return;
        }

        // may need to delay this to stop the double selecting
        StartCoroutine(DealyedSetSelected());
    }

    private IEnumerator DealyedSetSelected()
    {
        yield return new WaitForSeconds(0.2f);
        eventSystem.SetSelectedGameObject(uiToEnemy.gameObject);
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
    public void ToggleAbilityMenu(bool active, List<Ability> abilites = null, AbilityType type = AbilityType.ATTACK, bool sameChar = false)
    {
        selectAbilityMenu.SetActive(active);
        if(!active)
        { return; }
        abilityMenu.PopulateAbilityList(abilites, type, sameChar);
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
    
    public void EnableGameOver()
    {
        StartCoroutine(GameOver());
    }

    private IEnumerator GameOver()
    {
        playerbattlemenu.SetActive(false);
        playerbattleHUD.SetActive(false);
        foreach (Animator animators in _engine.playerAnimators)
        {
            animators.GetComponent<SpriteRenderer>().sortingLayerName = "Forground";
        }
        gameoverfade.FadeOut();
        yield return new WaitForSeconds(2f);
        playerGameOverPanel.SetActive(true);
        eventSystem.SetSelectedGameObject(ContinueButton);
    }

    public void ExitBattle()
    {
        playerWonPanel.SetActive(false);
    }
}
