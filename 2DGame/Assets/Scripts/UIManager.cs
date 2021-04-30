using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject fadeScreen;
    FadingScript fadeScriptObject;

    float fadeDura = 1f;
    float timeBeforeFadeIn = 0.5f;

    void Start()
    {
        fadeScriptObject = fadeScreen.GetComponent<FadingScript>();
    }

    public void ButtonOnClick(string sceneName)
    {
        if (sceneName == "DiningRoom")
        {
            fadeDura = 5f;
            timeBeforeFadeIn = 2.5f;
        }
        StartCoroutine(TransitionToScene(sceneName, fadeDura, timeBeforeFadeIn));
    }

    IEnumerator TransitionToScene(string sceneName, float duration, float timeBefore)
    {
        fadeScriptObject.BeginFade(1, duration);
        yield return new WaitForSeconds(duration + timeBefore);
        SceneManager.LoadScene(sceneName);
    }
}
