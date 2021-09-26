using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentScript : MonoBehaviour
{
    public PlayerScript player;
    public TileManager tileManager;

    private Animator mAnimator;
    private bool mAngry = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        mAnimator = GetComponent<Animator>();
        
        mAnimator.Play("ParentWalkingAnimation");
    }

    public void MoveCloser()
    {
        transform.position += new Vector3(player.tileSize.x, 0, 0);
    }

    public void SwitchUp()
    {
        transform.position += new Vector3(0, player.tileSize.y, 0);
    }
    
    public void SwitchDown()
    {
        transform.position -= new Vector3(0, player.tileSize.y, 0);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (tileManager.getPlayerTile(transform.position) == TileManager.TileType.OBSTACLE && !mAngry)
        {
            mAngry = true;
            mAnimator.Play("ParentAngryAnimation");

            StartCoroutine("UnAngry");
        }
    }

    private IEnumerator UnAngry()
    {
        yield return new WaitForSeconds((45.0f * 2) / 60);

        mAngry = false;
        mAnimator.Play("ParentWalkingAnimation");
        
        StopCoroutine("UnAngry");
    }
}
