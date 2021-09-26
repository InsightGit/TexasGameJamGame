using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlideManager : MonoBehaviour
{
    public FadeTransitionManager fadeTransitionManager;
    public float timeOnSlide = 2.0f;
    public List<Sprite> slides;
    public String sceneNameToSwitchTo;
    
    private int mCurrentSlide = 0;
    private SpriteRenderer mSpriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        mSpriteRenderer = GetComponent<SpriteRenderer>();

        mSpriteRenderer.enabled = true;

        StartCoroutine("MainSlidesLoop");
    }

    private IEnumerator MainSlidesLoop()
    {
        while (mCurrentSlide < slides.Count)
        {
            fadeTransitionManager.StartFadeIn();

            mSpriteRenderer.sprite = slides[mCurrentSlide];

            yield return new WaitUntil(() => fadeTransitionManager.hasTransitionCompleted());

            yield return new WaitForSeconds(timeOnSlide);
        
            fadeTransitionManager.StartFadeOut();

            yield return new WaitUntil(() => fadeTransitionManager.hasTransitionCompleted());

            ++mCurrentSlide;
        }

        SceneManager.LoadScene(sceneNameToSwitchTo);

        StopCoroutine("MainSlidesLoop");
    }
}
