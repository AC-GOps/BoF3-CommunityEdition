using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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

    public bool spamBlock;

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


        if(!clearing)
        {
            return;
        }
        ClearText();
    }

    public void SetDialouge(DialogueScript Dscript)
    {
        script = Dscript;
    }

    public void GetInput(InputAction.CallbackContext context)
    {
        if (spamBlock)
        {
            return;
        }

        if (context.performed)
        {
            spamBlock = true;
            if (printing)
            {
                textSpeed = 0;
                return;
            }

            if (currentDialogue == script.dialogueText.Count - 1)
            {
                CloseTextBox();
                return;
            }

            clearing = true;
        }
    }

    public void SpawnNextDialogue()
    {
        spamBlock = false;
        textBoxOpen = true; // this has to be called at the end of the text box opening
        currentDialogue++;

        if (currentDialogue >= script.dialogueText.Count)
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
        spamBlock = true;
        if (offset < -1)
        {
            offset = -1;
            spamBlock = false;
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
            spamBlock = false;
            yield break;
        }

        currentCharIndex++;
        StartCoroutine(PrintText());
    }

    public void OpenTextBox()
    {
        spamBlock = true;

        if(interactionManager.interactableObject!=null)
        {
            interactionManager.interactableObject.onOpen.Invoke();
        }

        textBox.gameObject.SetActive(true);
        text.text = "";
        currentDialogue = -1;
        textBox.DOScale(Vector3.one, 1 / textBoxSpeed).onComplete = SpawnNextDialogue;
    }

    public void CloseTextBox()
    {
        textBox.DOScale(Vector3.zero, 1 / textBoxSpeed).OnComplete(DeActiveate);
        textBoxOpen = false;
        if (interactionManager.interactableObject != null)
        {
            interactionManager.interactableObject.onClose.Invoke();
        }
    }

    public void DeActiveate()
    {
        textBox.gameObject.SetActive(false);
        spamBlock = false;
        if (interactionManager.interactableObject != null)
        {
            if (interactionManager.interactableObject.battleTriggerTest)
            {
                return;
            }
        }

        interactionManager.canInteract = true;
        PlayerInputManager.Instance.SwapActionMaps("Gameplay");
    }
}
