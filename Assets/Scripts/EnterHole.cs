using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterHole : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (MG2GameManager.wallApproaches < 40 && collision.name == "Player")
        {
            collision.transform.GetComponent<SpriteRenderer>().sortingOrder = -10;
        }
    }
}
