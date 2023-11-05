using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CutSceneManager : MonoBehaviour
{
    public static CutSceneManager instance;
    public List<UnityEvent> unityEvents;
    public int currentEvent;
    public int cutsceneMusic;

    private void Awake()
    {
        // Create a singleton instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        currentEvent = 0;
    }

    private void Start()
    {
        if (GameSingleton.Instance.cutSceneTriggered)
        {
            PlayerCharacterManager.instance.playerCharacterController.transform.position = AreaManager.instance.spawnLocation.position;
            return;
        }
        StartSequence();
        PlayerInputManager.Instance.SwapActionMaps("");
    }

    public void GetInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartSequence();
            PlayerInputManager.Instance.SwapActionMaps("");
        }
    }

    public void StartSequence()
    {
        unityEvents[currentEvent].Invoke();
    }

    public void OnCompleteEvent()
    {
        currentEvent++;
        if(currentEvent>unityEvents.Count-1)
        {
            EndofCutScene();
            return;
        }
        unityEvents[currentEvent].Invoke();
    }

    public void StartCutSceneMusic()
    {
        AudioManager.instance.PlayMusic(cutsceneMusic);
    }

    public void EndofCutScene()
    {
        print("end of sequence");
        GameSingleton.Instance.cutSceneTriggered = true;
    }
}
