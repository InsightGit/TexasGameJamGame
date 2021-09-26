using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentScript : MonoBehaviour
{
    public PlayerScript player;
    
    // Start is called before the first frame update
    void Start()
    {
       // 
    }

    public void MoveCloser()
    {
        transform.position += new Vector3(player.tileSize.x, 0, 0);
    }
    
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, player.transform.position.y, 0);
    }
}
