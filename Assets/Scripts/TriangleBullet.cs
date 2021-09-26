using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleBullet : MonoBehaviour
{
    public GameObject particleTri;
    public int podType;
    public GameObject smallerTriangle;
    public bool leftRight = true;

    // Start is called before the first frame update
    void Start()
    {
        if (podType == 0)
        {
            leftRight = (Random.Range(0, 2) == 0);
            transform.position = new Vector3(39 * (leftRight ? -1 : 1), Random.Range(-10, 10), 0);
        }
        if (podType == 1)
        {
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(MG1PlayerMovement.mainPlayer.position.y - transform.position.y,
                MG1PlayerMovement.mainPlayer.position.x - transform.position.x) * Mathf.Rad2Deg);
        }
        if (podType == 2)
        {
            if (transform.childCount > 0)
                transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = Random.Range(0, 2) == 0;
            transform.GetComponent<Rigidbody2D>().velocity += (Vector2)transform.right * 30;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (podType == 1)
            transform.position += transform.right * Time.deltaTime * 85;
        if (podType == 2 && transform.childCount > 0)
        {
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, Mathf.Atan2(GetComponent<Rigidbody2D>().velocity.y, GetComponent<Rigidbody2D>().velocity.x) * Mathf.Rad2Deg + 90);
        }
    }

    public void shootProjectile ()
    {
        GameObject triangleAttack = Instantiate(smallerTriangle, transform.position, Quaternion.identity);
        triangleAttack.GetComponent<TriangleBullet>().leftRight = leftRight;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            MG1PlayerMovement.health -= (podType == 1 ? 2 : 1);
            Destroy(gameObject);
        }
        else if (podType == 1 && ((collision.name != "LeftBounds" && leftRight) || (collision.name != "RightBounds" && !leftRight)) && !collision.name.StartsWith("Teardrop") && !collision.name.StartsWith("Attack1"))
        {
            for (int i = 0; i < 3; i++)
                Instantiate(smallerTriangle, transform.position + Vector3.down * .25f, Quaternion.Euler(0, 0, 110 - i * 20 + Random.Range(-5, 5)));
            Destroy(Instantiate(particleTri, transform.position, Quaternion.identity), 2);
            Destroy(gameObject);
        }
        else if (podType == 2 && !collision.name.StartsWith("Attack1") && !collision.name.StartsWith("RoofBounds"))
            StartCoroutine(fadeTriangle());
    }

    IEnumerator fadeTriangle()
    {
        podType = 3;
        for (int i = 0; i < 10; i++)
        {
            GetComponentInChildren<SpriteRenderer>().color -= new Color(0, 0, 0, .1f);
            yield return null;
        }
        Destroy(gameObject);
    }
}
