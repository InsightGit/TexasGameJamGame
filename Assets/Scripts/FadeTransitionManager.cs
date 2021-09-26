using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeTransitionManager : MonoBehaviour
{
    public int transitionFrameLength = 60;
    
    private int mCurrentFrame = 60;
    private bool mFadingIn = false;
    private SpriteRenderer mSpriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        mSpriteRenderer = GetComponent<SpriteRenderer>();
        
        mSpriteRenderer.material.color = Color.clear;
        mSpriteRenderer.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasTransitionCompleted())
        {
            Color newColor;
            float lerpT = (float)mCurrentFrame / transitionFrameLength;
            
            if (mFadingIn)
            {
                newColor = Color.Lerp(Color.black, Color.clear, lerpT);
            }
            else
            {
                newColor = Color.Lerp(Color.clear, Color.black, lerpT);
            }

            mSpriteRenderer.material.color = newColor;

            ++mCurrentFrame;
        }
    }

    public void StartFadeIn()
    {
        mCurrentFrame = 0;
        mFadingIn = true;
        mSpriteRenderer.material.color = Color.black;
    }

    public void StartFadeOut()
    {
        mCurrentFrame = 0;
        mFadingIn = false;
        mSpriteRenderer.material.color = Color.clear;
    }

    public bool hasTransitionCompleted()
    {
        return transitionFrameLength <= mCurrentFrame;
    }
}
