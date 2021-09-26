using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public String playTargetSceneString = "StorySlideScene";
    public String tileRunnerSceneString = "TileRunner";
    public FadeTransitionManager fadeTransitionManager;

    public List<AudioClip> introAudioClips;
    public List<Sprite> introSlides;

    private bool mFadeOutStarted = false;

    // Update is called once per frame
    void Update()
    {
        if (fadeTransitionManager.hasTransitionCompleted() && mFadeOutStarted)
        {
            GameState.slideManagerAudioClips = introAudioClips;
            GameState.slideManagerSlides = introSlides;
            GameState.slideManagerTargetScene = tileRunnerSceneString;
            GameState.slideManagerSkipFirstFadeIn = true;
            
            SceneManager.LoadScene(playTargetSceneString);
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
