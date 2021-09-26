using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public String tileRunnerSceneString; 
    public FadeTransitionManager fadeTransitionManager;

    private bool mFadeOutStarted = false;

    // Update is called once per frame
    void Update()
    {
        if (fadeTransitionManager.hasTransitionCompleted() && mFadeOutStarted)
        {
            SceneManager.LoadScene(tileRunnerSceneString);
        }
    }

    public void onQuitButtonPressed()
    {
        Application.Quit();
    }

    public void onPlayGameButtonPressed()
    {
        mFadeOutStarted = true;
        
        fadeTransitionManager.StartFadeOut();
    }
}
