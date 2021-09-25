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
    public TileBase grassTile;
    public List<int> tileProbablities;

    private Tilemap mTilemap;
    
    // Start is called before the first frame update
    void Start()
    {
        mTilemap = GetComponent<Tilemap>();
        
        Assert.AreEqual(tilesToUse.Count, tileProbablities.Count);

        StartCoroutine("AddNewAndShiftTileStrips");
    }

    private IEnumerator AddNewAndShiftTileStrips()
    {
        while (true)
        {
            Vector3Int currentTileStripHead = firstTileStripPosition;

            Queue<TileBase> pastTileStrip = new Queue<TileBase>();

            for(int y = 0; tileStripLength / tileSize.y > y; ++y)
            {
                TileBase randomTile = null;

                for (int i = 0; tileProbablities.Count > i; ++i)
                {
                    if (Random.Range(0, tileProbablities[i]) == 0)
                    {
                        randomTile = tilesToUse[i];
                        break;
                    }
                }

                if (randomTile == null)
                {
                    randomTile = grassTile;
                }
                
                pastTileStrip.Enqueue(randomTile);
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

            yield return new WaitForSeconds(frameStripUpdateSpeedInSeconds);
        }
    }
}
