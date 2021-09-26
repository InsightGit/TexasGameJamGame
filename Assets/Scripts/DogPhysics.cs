using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogPhysics : MonoBehaviour
{
    public GameObject heartParticles;
    public Sprite[] dogsAngry;
    public Sprite[] dogsHappy;
    int dogNum = 0;
    public int pet = 0;
    public int touching = 0;

    // Start is called before the first frame update
    void Start()
    {
        dogNum = Random.Range(0, 5);
        GetComponentInChildren<SpriteRenderer>().sprite = dogsAngry[dogNum];
        if (MG3GameManager.startGame)
            StartCoroutine(dogRandomness());
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.layer == 8)
            GetComponent<Rigidbody2D>().velocity -= Vector2.right * 75f * Time.deltaTime;
        if (transform.position.x > 15)
            GetComponent<Rigidbody2D>().velocity -= Vector2.right * 15f * Time.deltaTime;
        if (pet == 4 && gameObject.layer != 8)
        {
            GetComponentInChildren<SpriteRenderer>().sprite = dogsHappy[dogNum];
            gameObject.layer = 8;
            for (int i = 0; i < 8; i++)
                displayHearts();
        }
    }

    public IEnumerator dogRandomness ()
    {
        while (pet < 4)
        {
            int waitTime = Random.Range(100, 10000);
            for (int i = 0; i < waitTime; i++)
            {
                yield return null;
                if (pet > 3)
                    break;
            }
            GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-30, 30), Random.Range(5, 25)) * 2;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.name == "Player")
            StartCoroutine(touchDelay());
    }

    IEnumerator touchDelay ()
    {
        touching++;
        yield return new WaitForSeconds(.1f);
        touching--;
    }

    public void displayHearts()
    {
        Destroy(Instantiate(heartParticles, transform.position, Quaternion.identity), 5);
    }
}
