using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMenu : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject QuitButton;
    public EventSystem eventSystem;
    public List<PlayerMenuBox> playerBox = new List<PlayerMenuBox>();

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleMenu(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            bool active = !PauseMenu.activeSelf;
            PauseMenu.SetActive(active);
            UpdatePlayerBoxs();
            PlayerCharacterManager.instance.playerCharacterController.enabled = !active;
            if (PauseMenu.activeSelf)
            {
                eventSystem.SetSelectedGameObject(QuitButton);
            }
        }
    }

    public void ToggleMenu()
    {
        bool active = !PauseMenu.activeSelf;
        PauseMenu.SetActive(active);
        PlayerCharacterManager.instance.playerCharacterController.enabled = !active;
        if (PauseMenu.activeSelf)
        {
            eventSystem.SetSelectedGameObject(QuitButton);
        }
    }

    public void UpdatePlayerBoxs()
    {
        foreach(PlayerMenuBox box in playerBox)
        {
            box.gameObject.SetActive(false);
        }
        var PCM = PlayerCharacterManager.instance;
        for (int i = 0; i < PCM.playerBattleCharacters.Count; i++)
        {
            playerBox[i].gameObject.SetActive(true);
            playerBox[i].UpdateInfo(PCM.playerBattleCharacters[i]);
        }
    }
}
