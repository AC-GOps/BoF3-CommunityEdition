using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance;
    public InputActionAsset inputAction;
    public PlayerInput playerInput;

    private void Awake()
    {
        // Create a singleton instance
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
        inputAction = playerInput.actions;
    }

    public void SwapActionMaps(string actionmapName)
    {
        if (actionmapName == "")
        {
            playerInput.currentActionMap = null;
            return;
        }

        //InputActionMap actionmap = playerInput.actions.FindActionMap(actionmapName);

        print(actionmapName + " is now Active");
        playerInput.SwitchCurrentActionMap(actionmapName);
    }
}
