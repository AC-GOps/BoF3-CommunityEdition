using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Narrator : MonoBehaviour
{
    public static Narrator Instance { get; private set; }

    public IsometricCharacterController controller;
    public InteractionManager interactionManager;

    public RectTransform textBox;
    public TMP_Text text;
    public int currentDialogue;
    public DialogueScript script;

    public float textSpeed;
    public float textBoxSpeed;
    public float clearSpeed;
    private float _savedTextSpeed;
    private int currentCharIndex;

    public string newText;
    private float time;

    public bool printing;
    public bool clearing;
    public bool textBoxOpen;

    public float offset;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentDialogue = -1;
        offset = 0.1f;
        _savedTextSpeed = textSpeed;
    }

    private void Update()
    {
        if(!textBoxOpen)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.X))
        {
            if (printing)
            {
                textSpeed = 0;
                return;
            }

            if (currentDialogue == script.dialogueText.Count-1)
            {
                CloseTextBox();
                return;
            }

            clearing = true;
        }


        if(!clearing)
        {
            return;
        }
        ClearText();
    }

    public void SpawnNextDialogue()
    {
        
        textBoxOpen = true; // this has to be called at the end of the text box opening
        currentDialogue++;

        if (currentDialogue == script.dialogueText.Count)
        {
            return;
        }


        offset = 0.1f;
        text.fontMaterial.SetTextureOffset("_FaceTex", new Vector2(0, 1));
        text.fontMaterial.SetTextureOffset("_OutlineTex", new Vector2(0, 1));
        newText = script.dialogueText[currentDialogue];
        printing = true;
        currentCharIndex = 0;
        textSpeed = _savedTextSpeed;
        StartCoroutine(PrintText());
    }

    public void ClearText()
    {
        if (offset < -1)
        {
            offset = -1;
            clearing = false;
            text.text = "";
            SpawnNextDialogue();
            return;
        }
        offset -= Time.deltaTime * clearSpeed;
        text.fontMaterial.SetTextureOffset("_FaceTex", new Vector2(0, offset));
        text.fontMaterial.SetTextureOffset("_OutlineTex", new Vector2(0, offset));
    }

    private IEnumerator PrintText()
    {
        yield return new WaitForSeconds(textSpeed);
        char c = newText[currentCharIndex];

        if (char.IsSymbol(c))
        {
            text.text += "\n";
            currentCharIndex++;
            StartCoroutine(PrintText());
            yield break;
        }

        text.text += newText[currentCharIndex];
        if (currentCharIndex == newText.Length-1)
        {
            printing = false;
            yield break;
        }

        currentCharIndex++;
        StartCoroutine(PrintText());
    }

    public void OpenTextBox()
    {
        textBox.gameObject.SetActive(true);
        text.text = "";
        currentDialogue = -1;
        controller.canMove = false;
        interactionManager.canInteract = false;
        textBox.DOScale(Vector3.one, 1 / textBoxSpeed).onComplete = SpawnNextDialogue;
    }

    public void CloseTextBox()
    {
        textBox.DOScale(Vector3.zero, 1 / textBoxSpeed).OnComplete(DeActiveate);
        textBoxOpen = false;
        controller.canMove = true;
        interactionManager.canInteract = true;

        interactionManager.interactableObject.onUsed.Invoke();

        if (interactionManager.interactableObject.battleTriggerTest)
        {
            interactionManager.canInteract = false;
            interactionManager.interactableObject.GetComponentInParent<BattleCharacter>().gameObject.SetActive(false);
            interactionManager.interactableObject = null;
            TurnBasedBattleEngine.Instance.Init();
        }

    }

    public void DeActiveate()
    {
        textBox.gameObject.SetActive(false);
    }
}
