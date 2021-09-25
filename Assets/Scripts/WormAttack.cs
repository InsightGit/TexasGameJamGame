using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormAttack : MonoBehaviour
{
    public bool leftRight = true;
    public static bool hitPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.GetSiblingIndex() == 0)
        {
            hitPlayer = false;
            leftRight = (Random.Range(0, 2) == 0);
            transform.parent.position = new Vector3(56.7f * (leftRight ? 1 : -1), -15f, 0);
            transform.parent.localScale = new Vector3((leftRight ? 1 : -1), 1, 1);
            foreach (WormAttack wa in transform.parent.GetComponentsInChildren<WormAttack>())
                wa.leftRight = leftRight;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.name.StartsWith("Circle"))
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(20 * (leftRight ? -1 : 1), 40);
        }
        if (collision.transform.name == "Player" && !hitPlayer)
        {
            MG1PlayerMovement.health -= 2;
            hitPlayer = true;
        }
    }
}
