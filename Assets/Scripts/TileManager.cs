using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public Vector3Int firstTileStripPosition;
    public Vector2Int tileSize = new Vector2Int(20, 20);
    public int numberOfTileStrips = 10;
    public int tileStripLength = 100;
    public float frameStripUpdateSpeedInSeconds = 1.0f;
    public List<TileBase> tilesToUse;
    public List<TileBase> obstacleTiles;
    public TileBase grassTile;
    public TileBase minigameTile;
    public List<int> tileProbablities;
    public int maxWavesWithoutMinigames = 20;

    private Tilemap mTilemap;
    private int mWavesWithoutMinigames = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        mTilemap = GetComponent<Tilemap>();
        
        Assert.AreEqual(tilesToUse.Count, tileProbablities.Count);

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

                if (obstacleTiles.Contains(pastTile) && obstacleTiles.Contains(tilesToUse[i]))
                {
                    continue;
                }
                else
                {
                    randomTile = tilesToUse[i];
                }
                        
                break;
            }
        }

        if (randomTile == null)
        {
            randomTile = grassTile;
        }

        return randomTile;
    }
    
    private IEnumerator AddNewAndShiftTileStrips()
    {
        while (true)
        {
            Vector3Int currentTileStripHead = firstTileStripPosition;

            Queue<TileBase> pastTileStrip = new Queue<TileBase>();

            for(int y = 0; tileStripLength / tileSize.y > y; ++y)
            {
                if (mWavesWithoutMinigames < maxWavesWithoutMinigames)
                {
                    pastTileStrip.Enqueue(generateRandomTile(currentTileStripHead, y));
                }
                else
                {
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

            ++mWavesWithoutMinigames;
            
            yield return new WaitForSeconds(frameStripUpdateSpeedInSeconds);
        }
    }
}
