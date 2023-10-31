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
    // Update is called once per frame


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
}
