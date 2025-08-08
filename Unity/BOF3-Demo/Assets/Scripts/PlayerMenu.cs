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
    public List<PlayerMenuBox> playerboxes = new List<PlayerMenuBox>();


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
            PlayerCharacterManager.instance.playerCharacterController.enabled = !active;
            if (PauseMenu.activeSelf)
            {
                //eventSystem.SetSelectedGameObject(QuitButton);
                UpdatePlayerBoxes();
            }
        }
    }

    private void UpdatePlayerBoxes()
    {
        var players = PlayerCharacterManager.instance.playerBattleCharacters;
        foreach(PlayerMenuBox box in playerboxes)
        {
            box.gameObject.SetActive(false);
        }
        for (int i = 0; i < players.Count; i++)
        {
            playerboxes[i].UpdateInfo(players[i]);
            playerboxes[i].gameObject.SetActive(true);
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
}
