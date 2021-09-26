using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlideManager : MonoBehaviour
{
    public FadeTransitionManager fadeTransitionManager;
    public float timeOnSlide = 5.0f;

    private AudioSource mAudioSource;
    private List<AudioClip> mSlideAudioClips;
    private int mCurrentSlide = 0;
    private List<Sprite> mSlides;
    private SpriteRenderer mSpriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
        mSpriteRenderer = GetComponent<SpriteRenderer>();

        mSpriteRenderer.enabled = true;
        mSlides = GameState.slideManagerSlides;
        mSlideAudioClips = GameState.slideManagerAudioClips;

        GameState.slideManagerAudioClips = new List<AudioClip>();

        StartCoroutine("MainSlidesLoop");
    }

    private IEnumerator MainSlidesLoop()
    {
        while (mCurrentSlide < mSlides.Count)
        {

            if (mCurrentSlide != 0 || !GameState.slideManagerSkipFirstFadeIn)
            {
                fadeTransitionManager.StartFadeIn();    
            }

            mSpriteRenderer.sprite = mSlides[mCurrentSlide];

            yield return new WaitUntil(() => fadeTransitionManager.hasTransitionCompleted());
            
            if (mCurrentSlide < mSlideAudioClips.Count)
            {
                mAudioSource.Stop();

                mAudioSource.clip = mSlideAudioClips[mCurrentSlide];
                
                mAudioSource.Play();
            }

            yield return new WaitForSeconds(timeOnSlide);
        
            fadeTransitionManager.StartFadeOut();

            yield return new WaitUntil(() => fadeTransitionManager.hasTransitionCompleted());

            ++mCurrentSlide;
        }

        mAudioSource.Stop();
        
        SceneManager.LoadScene(GameState.slideManagerTargetScene);

        GameState.slideManagerSkipFirstFadeIn = false;
        
        StopCoroutine("MainSlidesLoop");
    }
}
