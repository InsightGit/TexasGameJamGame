using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TileManager : MonoBehaviour
{
    public enum TileType
    {
        MINIGAME,
        GRASS,
        OBSTACLE
    }

    public enum ColorMode
    {
        BLUEANDRED,
        BLUEREDTRANSITION,
        INDIGOANDMAGENTA,
        INDIGOMAGENTATRANSITION,
        PURPLE
    }
    
    public Vector3Int firstTileStripPosition;
    public Vector2Int tileSize = new Vector2Int(20, 20);
    public int numberOfTileStrips = 10;
    public int tileStripLength = 100;
    public float frameStripUpdateSpeedInSeconds = 1.0f;
    public List<int> tileProbablities;
    public int transitionColorWaves = 5;
    public int maxWavesWithoutMinigames = 20;


    public List<TileBase> blueRedTiles;
    public List<TileBase> blueRedTransitionTiles;
    public List<TileBase> indigoMagentaTiles;
    public List<TileBase> indigoMagentaTransitionTiles;
    public TileBase purpleTile;

    public TileBase blueForestTile;
    public TileBase redForestTile;

    public TileBase blueObstacleTile;
    public TileBase redObstacleTile;
    
    public TileBase blueMinigameTile;
    public TileBase redMinigameTile;
    
    private ColorMode mColorMode = ColorMode.BLUEANDRED;
    private Tilemap mTilemap;
    private int mWavesWithoutMinigames = 0;

    private bool isMinigameTile(TileBase tile)
    {
        return blueMinigameTile == tile || redMinigameTile == tile;
    }
    
    private bool isObstacleTile(TileBase tile)
    {
        return blueObstacleTile == tile || redObstacleTile == tile;
    }
    
    TileBase getGrassTile(int linePosition)
    {
        int colorArrayIndex = -1;

        switch (linePosition)
        {
            case 0:
            case 1:
                colorArrayIndex = 2;
                break;
            case 2:
                colorArrayIndex = 1;
                break;
            case 3:
            case 4:
                colorArrayIndex = 0;
                break;
        }
        
        switch (mColorMode)
        {
            case ColorMode.BLUEANDRED:
                return blueRedTiles[colorArrayIndex];
            case ColorMode.BLUEREDTRANSITION:
                return blueRedTransitionTiles[colorArrayIndex];
            case ColorMode.INDIGOANDMAGENTA:
                return indigoMagentaTiles[colorArrayIndex];
            case ColorMode.INDIGOMAGENTATRANSITION:
                return indigoMagentaTransitionTiles[colorArrayIndex];
            case ColorMode.PURPLE:
                return purpleTile;
        }

        return null;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        mTilemap = GetComponent<Tilemap>();

        switch (GameState.minigamesCompleted)
        {
            case 0:
                mColorMode = ColorMode.BLUEANDRED;
                break;
            case 1:
                mColorMode = ColorMode.BLUEREDTRANSITION;
                break;
            case 2:
                mColorMode = ColorMode.INDIGOMAGENTATRANSITION;
                break;
        }

        for (int i = 0; numberOfTileStrips > i; ++i)
        {
            for (int y = 0; tileStripLength > y; ++y)
            {
                for (int x = 0; tileSize.x > x; ++x)
                {
                    Vector3Int tilePosition = new Vector3Int(firstTileStripPosition.x - (i * tileSize.x) - x, 
                        firstTileStripPosition.y + y, 0);
                    
                    mTilemap.SetTile(tilePosition, getGrassTile(y / tileSize.y));
                }
            }
        }
        
        StartCoroutine("AddNewAndShiftTileStrips");
    }

    private TileBase generateRandomTile(Vector3Int currentTileStripHead, int tileY)
    {
        TileBase randomTile = null;
        
        for (int i = 0; tileProbablities.Count > i; ++i)
        {
            if (Random.Range(0, tileProbablities[i]) == 0)
            {
                Vector3Int pastTilePosition = new Vector3Int(currentTileStripHead.x, 
                    currentTileStripHead.y + (tileY * tileSize.y), 0);
                TileBase pastTile = mTilemap.GetTile(pastTilePosition);

                if (isObstacleTile(pastTile))
                {
                    continue;
                }
                else
                {
                    if (tileY > 2)
                    {
                        randomTile = blueObstacleTile;
                    }
                    else
                    {
                        randomTile = redObstacleTile;
                    }

                    break;
                }
            }
        }

        if (randomTile == null)
        {
            randomTile = getGrassTile(tileY);
        }

        return randomTile;
    }
    
    private IEnumerator AddNewAndShiftTileStrips()
    {
        while (true)
        {
            if (GameState.paused)
            {
                StopCoroutine("AddNewAndShiftTileStrips");
            }
            
            Vector3Int currentTileStripHead = firstTileStripPosition;

            Queue<TileBase> pastTileStrip = new Queue<TileBase>();

            if (mWavesWithoutMinigames > transitionColorWaves)
            {
                switch (mColorMode)
                {
                    case ColorMode.BLUEREDTRANSITION:
                        mColorMode = ColorMode.INDIGOANDMAGENTA;
                        break;
                    case ColorMode.INDIGOMAGENTATRANSITION:
                        mColorMode = ColorMode.PURPLE;
                        break;
                }
            }
            
            for(int y = 0; tileStripLength / tileSize.y > y; ++y)
            {
                if (mWavesWithoutMinigames < maxWavesWithoutMinigames)
                {
                    pastTileStrip.Enqueue(generateRandomTile(currentTileStripHead, y));
                }
                else
                {
                    TileBase minigameTile;
                    
                    if (y > 2)
                    {
                        minigameTile = redMinigameTile;
                    }
                    else
                    {
                        minigameTile = blueMinigameTile;
                    }
                    
                    pastTileStrip.Enqueue(minigameTile);
                }
            }
        
            TileBase currentTile = null;
        
            for (int i = 0; numberOfTileStrips > i; ++i)
            {
                Assert.AreEqual(tileStripLength / tileSize.y, pastTileStrip.Count);
                
                for (int y = 0; tileStripLength > y; ++y)
                {
                    if (y % tileSize.y == 0)
                    {
                        Vector3Int tilePosition = new Vector3Int(currentTileStripHead.x - (i * tileSize.x), 
                                                                 currentTileStripHead.y + y, 0);
                        TileBase tile = mTilemap.GetTile(tilePosition);
                        
                        currentTile = pastTileStrip.Dequeue();

                        pastTileStrip.Enqueue(tile);
                    }
                
                    for (int x = 0; tileSize.x > x; ++x)
                    {
                        Vector3Int tilePosition = new Vector3Int(currentTileStripHead.x - (i * tileSize.x) - x, 
                            currentTileStripHead.y + y, 0);
                    
                        mTilemap.SetTile(tilePosition, currentTile);
                    }
                }
            }

            // Forest generation
            for (int i = 0; numberOfTileStrips > i; ++i)
            {
                for (int y = 0; tileSize.y > y; ++y)
                {
                    for (int x = 0; tileSize.x > x; ++x)
                    {
                        Vector3Int blueForestPosition = new Vector3Int(currentTileStripHead.x - (i * tileSize.x) - x, 
                            currentTileStripHead.y + tileStripLength - y, 0);
                        Vector3Int redForestPosition = new Vector3Int(currentTileStripHead.x - (i * tileSize.x) - x, 
                            currentTileStripHead.y - y, 0);
                    
                        mTilemap.SetTile(blueForestPosition, blueForestTile);
                        mTilemap.SetTile(redForestPosition, redForestTile);
                    }
                }
            }

            ++mWavesWithoutMinigames;
            
            yield return new WaitForSeconds(frameStripUpdateSpeedInSeconds);
        }
    }

    public TileType getPlayerTile(Vector3 playerPosition)
    {
        Vector3Int cellPosition = mTilemap.WorldToCell(playerPosition);
        TileBase playerTile = mTilemap.GetTile(cellPosition);

        if (isMinigameTile(playerTile))
        {
            return TileType.MINIGAME;
        }
        else if(isObstacleTile(playerTile))
        {
            return TileType.OBSTACLE;
        }
        else
        {
            return TileType.GRASS;
        }
    }
}
