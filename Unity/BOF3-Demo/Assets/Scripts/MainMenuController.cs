using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    public SceneChanger sceneChanger;
    public AudioClip EnterGame;
    public FlashText flashText;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        //AudioManager.instance.PlayMusic(0);
    }

    public void StartGame()
    {
        flashText.EndFlash();
        AudioManager.instance.PlaySFXFromClip(EnterGame);
        sceneChanger.LoadSceneBynameAsync("Forest");
    }
}
