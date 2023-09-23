using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    public FadeImage fade;
    public AudioSource BG;
    public IEnumerator StartGame()
    {
        fade.FadeIn();
        BG.DOFade(0, 0.5f);
        yield return new WaitForSeconds(fade.Time);
        SceneChanger.LoadSceneByname("DemoScene");
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
    }

    private void Update()
    {
        if(Input.anyKeyDown)
        {
            if(Input.GetMouseButtonDown(0))
            {
                return;
            }
          StartCoroutine(StartGame());
        }
    }
}
