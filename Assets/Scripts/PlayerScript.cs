using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlayerScript : MonoBehaviour
{
    public Vector2 tileSize = new Vector2(1.28f, 1.28f);

    public FadeTransitionManager fadeTransitionManager;
    public ParentScript parentEnemy;
    public String parentMinigameScene = "MGBulletDodge";
    public String slidesSceneName = "StorySlideScene";
    public String gameOverSceneName = "TheEnd";
    public TileManager tileManager;
    
    public float jumpingLengthSeconds = 0.8167f;
    public int maxObstacleHitsBeforeParentMinigame = 3;

    public List<Sprite> checkpointMinigameSlides;
    public List<Sprite> parentMinigameSlides;
    public List<Sprite> gameOverSlides;
    
    public List<String> checkpointMinigames;

    private Animator mAnimator;
    private int mCurrentTileStrip;
    private Vector2 mMovement;
    private int mObstacleHits = 0;
    private bool mJumping = false;
    private AudioSource mJumpAudioSource;
    private TileManager.TileType mPastTileType = TileManager.TileType.GRASS;

    // Start is called before the first frame update
    void Start()
    {
        mAnimator = GetComponent<Animator>();
        mCurrentTileStrip = (tileManager.tileStripLength / tileManager.tileSize.y) / 2;
        mJumpAudioSource = GetComponent<AudioSource>();
        mObstacleHits = GameState.obstaclesHits;

        for (int i = 0; mObstacleHits > i; ++i)
        {
            parentEnemy.MoveCloser();
        }
        
        mAnimator.Play("PlayerWalkingAnimation");
    }

    // Update is called once per frame
    void Update()
    {
        if (!mJumping && !GameState.paused)
        {
            TileManager.TileType currentTile = tileManager.getPlayerTile(transform.position);

            if (currentTile != mPastTileType)
            {
                switch (currentTile)
                {
                    case TileManager.TileType.MINIGAME:
                        fadeTransitionManager.StartFadeOut();

                        if (GameState.minigamesCompleted >= checkpointMinigames.Count)
                        {
                            GameState.slideManagerTargetScene = gameOverSceneName;
                            GameState.slideManagerSlides = gameOverSlides;
                        }
                        else
                        {
                            GameState.slideManagerTargetScene = checkpointMinigames[GameState.minigamesCompleted]; 
                            GameState.slideManagerSlides = checkpointMinigameSlides;
                        }
                        
                        GameState.paused = true;
                        break;
                    case TileManager.TileType.GRASS:
                        break;
                    case TileManager.TileType.OBSTACLE:
                        ++mObstacleHits;

                        parentEnemy.MoveCloser();
                        
                        if (maxObstacleHitsBeforeParentMinigame > mObstacleHits)
                        {
                            Debug.Log("The parent is getting closer!");
                        }
                        else
                        {
                            Debug.Log("Parent Minigame time!");
                            
                            fadeTransitionManager.StartFadeOut();

                            GameState.slideManagerTargetScene = parentMinigameScene;
                            GameState.slideManagerSlides = parentMinigameSlides;
                            GameState.paused = true;
                        }
                        break;
                }
                
                mPastTileType = currentTile;
            }

            if (Input.GetButtonDown("Jump"))
            {
                mJumping = true;
                
                transform.position += new Vector3(0, tileSize.y / 2, 0);

                mAnimator.Play("PlayerJumpingAnimation");
                mJumpAudioSource.Play();
                
                StartCoroutine("endJumping");
            }
            else if(Input.GetButtonDown("Binary Up") && mCurrentTileStrip > 0)
            {
                --mCurrentTileStrip;

                transform.position += new Vector3(0, tileSize.y, 0);
            } 
            else if (Input.GetButtonDown("Binary Down") && mCurrentTileStrip < 
                ((tileManager.tileStripLength / tileManager.tileSize.y) - 1))
            {
                ++mCurrentTileStrip;
                
                transform.position -= new Vector3(0, tileSize.y, 0);
            }
        } 
        else if (GameState.paused && fadeTransitionManager.hasTransitionCompleted())
        {
            GameState.paused = false;
            GameState.obstaclesHits = mObstacleHits;
            
            SceneManager.LoadScene(slidesSceneName);
        }
    }

    private IEnumerator endJumping()
    {
        yield return new WaitForSeconds(jumpingLengthSeconds);

        transform.position -= new Vector3(0, tileSize.y / 2, 0);
        
        mJumping = false;
        
        mAnimator.Play("PlayerWalkingAnimation");
        
        StopCoroutine("endJumping");
    }
}
