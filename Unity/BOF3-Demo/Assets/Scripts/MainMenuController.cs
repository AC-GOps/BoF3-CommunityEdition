using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public FadeImage fade;
    public IEnumerator StartGame()
    {
        fade.Play();
        yield return new WaitForSeconds(fade.Time);
        SceneManager.LoadScene("DemoScene");
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
