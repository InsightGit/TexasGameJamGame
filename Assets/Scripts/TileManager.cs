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
    public int frameStripUpdateSpeed = 30;
    public List<TileBase> tilesToUse;

    private int mFrameNumber = 0;
    private Tilemap mTilemap;
    
    // Start is called before the first frame update
    void Start()
    {
        mTilemap = GetComponent<Tilemap>();
    }

    void AddNewAndShiftTileStrips()
    {
        Vector3Int currentTileStripHead = firstTileStripPosition;

        List<TileBase> newTileStrip = new List<TileBase>();
        Queue<List<TileBase>> tileStrips = new Queue<List<TileBase>>();

        for (int y = 0; tileStripLength / tileSize.y > y; ++y)
        {
            newTileStrip.Add(tilesToUse[Random.Range(0, tilesToUse.Count)]);
        }

        tileStrips.Enqueue(newTileStrip);
        
        for (int i = 0; numberOfTileStrips - 1 > i; ++i)
        {
            List<TileBase> tileStripLine = new List<TileBase>();
            
            for (int y = 0; tileStripLength / tileSize.y > y; ++y)
            {
                Vector3Int tilePosition = new Vector3Int(currentTileStripHead.x - (i * tileSize.x),
                    currentTileStripHead.y + (y * tileSize.y), 0);
                
                tileStripLine.Add(mTilemap.GetTile(tilePosition));
                //newTileStrip.Add();
            }
            
            tileStrips.Enqueue(tileStripLine);
        }
        
        Assert.AreEqual(numberOfTileStrips, tileStrips.Count);
        
        for(int i = 0; numberOfTileStrips > i; ++i)
        {
            List<TileBase> tileStripLine = tileStrips.Dequeue();
            
            for (int y = 0; tileStripLength > y; ++y)
            {
                TileBase currentTile = tileStripLine[y % tileSize.y];

                for (int x = 0; tileSize.x > x; ++x)
                {
                    Vector3Int tilePosition = new Vector3Int(currentTileStripHead.x - (i * tileSize.x),
                        currentTileStripHead.y + (y * tileSize.y), 0);
                    
                    mTilemap.SetTile(tilePosition, currentTile);
                }
            }
        }
    }
    
    
    // Update is called once per frame
    void Update()
    {
        ++mFrameNumber;

        if (frameStripUpdateSpeed >= mFrameNumber)
        {
            AddNewAndShiftTileStrips();
            mFrameNumber = 0;
        }
    }
    
            /*for (int i = 0; numberOfTileStrips > i; ++i)
        {
            currentTileStripHead.x = firstTileStripPosition.x - (tileSize.x * i);

            int pastTileId = 0;
            Vector2Int pastTileVector = new Vector2Int(-1, -1);

            for (int y = 0; tileStripLength > y; ++y)
            {
                Vector2Int tileVector = new Vector2Int(currentTileStripHead.x, y % tileSize.y);

                if (pastTileVector != tileVector)
                {
                    Assert.IsTrue(tilesToUse.Count > 0);

                    if (i == 0)
                    {
                        pastTileId = Random.Range(0, tilesToUse.Count);
                        
                    }
                }
                
                
            }
        }*/
}
