using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickMovement : MonoBehaviour
{
    Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        ramdomizePosition();
    }

    private void Update()
    {
        GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, .5f, (Mathf.Max(5, Mathf.Min(10, MG2GameManager.wallApproaches)) - 5) * .2f);
    }

    public void ramdomizePosition ()
    {
        transform.position = new Vector3(startPos.x + Random.Range(-5f, 5f), startPos.y + Random.Range(-.5f, .5f));
    }
}
