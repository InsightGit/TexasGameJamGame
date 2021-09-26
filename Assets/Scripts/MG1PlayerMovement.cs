using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MG1PlayerMovement : MonoBehaviour
{
    public static Transform mainPlayer;
    public static int health = 12;
    public GameObject[] Attacks;
    Rigidbody2D rb;
    Transform healthBar;
    bool onFloor = true;
    bool startGame = false;

    private void Start()
    {
        health = 12;
        FindObjectOfType<FadeTransitionManager>().StartFadeIn();
        mainPlayer = transform;
        rb = transform.GetComponent<Rigidbody2D>();
        healthBar = GameObject.Find("HealthReduction").transform;
        GameObject.Find("RandomBox").transform.localPosition = new Vector2(Random.Range(-20, -10f), -15);
        GameObject.Find("RandomBox (1)").transform.localPosition = new Vector2(Random.Range(10, 20f), -15);
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
            GetComponent<Animator>().SetBool("Jumping", true);
            onFloor = false;
            rbVelocity += Vector2.up * 55;
        }
        rb.velocity = new Vector2((rbVelocity.x + rb.velocity.x) / 2, rbVelocity.y);
        GetComponent<Animator>().SetBool("Moving", rb.velocity.x != 0);
        if (rb.velocity.x != 0)
            transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = rb.velocity.x < 0;
        healthBar.localPosition = new Vector3(0, health * .0833f);
        if (health <= 0 && health > -100)
        {
            health = -100;
            healthBar.transform.parent.GetChild(1).gameObject.SetActive(false);
            StartCoroutine(loadScene("MGBulletDodge"));
        }
    }

    IEnumerator loadScene (string sceneName)
    {
        FindObjectOfType<FadeTransitionManager>().StartFadeOut();
        while (!FindObjectOfType<FadeTransitionManager>().hasTransitionCompleted())
            yield return null;
        SceneManager.LoadScene(sceneName);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.name == "FloorBounds")
        {
            GetComponent<Animator>().SetBool("Jumping", false);
            onFloor = true;
        }
    }

    IEnumerator gameplay ()
    {
        StartCoroutine(timing());
        yield return new WaitForSeconds(2);
        GameObject.Find("PlayerAnim").GetComponent<Animator>().SetBool("Anger", true);
        Instantiate(Attacks[0]);
        yield return new WaitForSeconds(4);
        GameObject.Find("PlayerAnim").GetComponent<Animator>().SetBool("Attack", true);
        yield return new WaitForSeconds(1.7f);
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
        GameObject.Find("PlayerAnim").GetComponent<Animator>().SetBool("Anger", false);
        GameObject.Find("PlayerAnim").GetComponent<Animator>().SetBool("Attack", false);
        Instantiate(Attacks[0]);
        yield return new WaitForSeconds(.5f);
        GameObject.Find("PlayerAnim").GetComponent<Animator>().SetBool("Anger", true);
        Instantiate(Attacks[2]);
        yield return new WaitForSeconds(7);
        GameObject.Find("PlayerAnim").GetComponent<Animator>().SetBool("Attack", true);
        yield return new WaitForSeconds(.5f);
        for (int i = 0; i < 20; i++)
        {
            Instantiate(Attacks[1], GameObject.Find("TriangleParent").transform.position, Quaternion.Euler(0, 0, 90 + Random.Range(-30, 30)));
            yield return new WaitForSeconds(.5f);
            Instantiate(Attacks[1], GameObject.Find("CircleParent").transform.position, Quaternion.Euler(0, 0, 90 + Random.Range(-30, 30)));
            yield return new WaitForSeconds(.5f);
            if (i == 5)
                Instantiate(Attacks[2]);
            if (i == 10)
                Instantiate(Attacks[0]);
            if (i == 18)
            {
                GameObject.Find("PlayerAnim").GetComponent<Animator>().SetBool("Anger", false);
                GameObject.Find("PlayerAnim").GetComponent<Animator>().SetBool("Attack", false);
            }
        }
        yield return new WaitForSeconds(3.5f);
        StartCoroutine(loadScene("TileRunner"));
    }

    IEnumerator timing ()
    {
        Transform timingBar = GameObject.Find("TimeDeduction").transform;
        for (float i = 66; i >= 0; i -= Time.deltaTime)
        {
            timingBar.localPosition = Vector3.up * (i / 66f);
            yield return null;
        }
    }
}
