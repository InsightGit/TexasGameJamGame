using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerScript : MonoBehaviour
{
    public Vector2 tileSize = new Vector2(20 * 1.28f, 20 * 1.28f);
    public TileManager tileManager;
    public float jumpingLengthSeconds = 0.5f;

    private Vector2 mMovement;
    private bool mJumping = false;
    private int mCurrentTileStrip;

    // Start is called before the first frame update
    void Start()
    {
        mCurrentTileStrip = (tileManager.tileStripLength / tileManager.tileSize.y) / 2;
    }

    // Update is called once per frame
    void Update()
    {
        TileManager.TileType currentTile = tileManager.getPlayerTile(transform.position);

        switch (currentTile)
        {
            case TileManager.TileType.MINIGAME:
                Debug.Log("Checkpoint Minigame time!");
                break;
            case TileManager.TileType.GRASS:
                break;
            case TileManager.TileType.OBSTACLE:
                Debug.Log("Parent Minigame time!");
                break;
        }
        
        if (!mJumping)
        {
            if (Input.GetButtonDown("Jump"))
            {
                mJumping = true;
                
                transform.position += new Vector3(0, tileSize.y / 2, 0);

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
    }

    private IEnumerator endJumping()
    {
        yield return new WaitForSeconds(jumpingLengthSeconds);

        transform.position -= new Vector3(0, tileSize.y / 2, 0);
        
        mJumping = false;
        
        StopCoroutine("endJumping");
    }
}
