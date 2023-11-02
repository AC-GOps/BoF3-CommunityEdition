using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public FadeImage fade;

    public IEnumerator LoadSceneWithFade(string sceneName)
    {
        fade.FadeOut();
        yield return new WaitForSeconds(fade.Time +1);
        LoadSceneByname(sceneName);
    }

    public static void LoadSceneByname(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneBynameAsync(string sceneName)
    {
        StartCoroutine(LoadSceneWithFade(sceneName));
    }
}
