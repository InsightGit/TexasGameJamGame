using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG1PlayerMovement : MonoBehaviour
{
    public static Transform mainPlayer;
    public static int health = 10;
    public GameObject[] Attacks;
    Rigidbody2D rb;
    Transform healthBar;
    bool onFloor = true;
    bool startGame = false;

    // Start is called before the first frame update
    void Start()
    {
        mainPlayer = transform;
        rb = transform.GetComponent<Rigidbody2D>();
        healthBar = GameObject.Find("HealthReduction").transform;
        GameObject.Find("RandomBox").transform.localPosition = new Vector2(Random.Range(-15, 20), -15);
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !startGame)
        {
            Camera.main.GetComponent<Animator>().SetTrigger("StartGame");
            startGame = true;
            StartCoroutine(gameplay());
        }
        Vector2 rbVelocity = new Vector2(0, rb.velocity.y);
        if (Input.GetKey(KeyCode.A))
            rbVelocity -= Vector2.right * 25;
        if (Input.GetKey(KeyCode.D))
            rbVelocity += Vector2.right * 25;
        if (Input.GetKey(KeyCode.Space) && onFloor)
        {
            onFloor = false;
            rbVelocity += Vector2.up * 55;
        }
        rb.velocity = new Vector2((rbVelocity.x + rb.velocity.x) / 2, rbVelocity.y);
        healthBar.localPosition = new Vector3(0, health * .1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.name == "FloorBounds")
            onFloor = true;
    }

    IEnumerator gameplay ()
    {
        yield return new WaitForSeconds(2);
        Instantiate(Attacks[0]);
        yield return new WaitForSeconds(3);
        GameObject.Find("PlayerAnim").GetComponent<Animator>().SetBool("Attack", true);
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < 20; i++)
        {
            Instantiate(Attacks[1], GameObject.Find("CircleParent").transform.position, Quaternion.Euler(0, 0, 90 + Random.Range(-30, 30)));
            yield return new WaitForSeconds(.75f);
            Instantiate(Attacks[1], GameObject.Find("TriangleParent").transform.position, Quaternion.Euler(0, 0, 90 + Random.Range(-30, 30)));
            yield return new WaitForSeconds(.75f);
            if (i == 5)
                Instantiate(Attacks[2]);
            if (i == 15)
                Instantiate(Attacks[0]);
        }
        Instantiate(Attacks[0]);
        yield return new WaitForSeconds(.5f);
        Instantiate(Attacks[2]);
        yield return new WaitForSeconds(7);
        GameObject.Find("PlayerAnim").GetComponent<Animator>().SetBool("Attack", false);
        yield return new WaitForSeconds(.5f);
        for (int i = 0; i < 20; i++)
        {
            Instantiate(Attacks[1], GameObject.Find("CircleParent").transform.position, Quaternion.Euler(0, 0, 90 + Random.Range(-30, 30)));
            yield return new WaitForSeconds(.5f);
            Instantiate(Attacks[1], GameObject.Find("TriangleParent").transform.position, Quaternion.Euler(0, 0, 90 + Random.Range(-30, 30)));
            yield return new WaitForSeconds(.5f);
            if (i == 5)
                Instantiate(Attacks[2]);
            if (i == 10)
                Instantiate(Attacks[0]);
            if (i == 18)
                GameObject.Find("PlayerAnim").GetComponent<Animator>().SetBool("Attack", true);
        }
    }
}
