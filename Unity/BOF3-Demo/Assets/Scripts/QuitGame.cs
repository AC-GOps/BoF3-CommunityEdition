using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuitGame : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject QuitButton;
    public EventSystem eventSystem;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void QuitGameButton()
    {
        Application.Quit();
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
