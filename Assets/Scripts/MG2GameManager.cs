using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MG2GameManager : MonoBehaviour
{
    public static int wallApproaches = 50;
    public GameObject[] walls;
    bool startGame = false;
    Rigidbody2D rb;
    bool onFloor = true;

    // Start is called before the first frame update
    void Start()
    {
        wallApproaches = 50;
        FindObjectOfType<FadeTransitionManager>().StartFadeIn();
        rb = transform.GetComponent<Rigidbody2D>();
        walls[0].transform.GetChild(0).localPosition = new Vector3(Random.Range(-.4f, .4f), Random.Range(-2, 3) * .2f);
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
    }

    IEnumerator loadScene(string sceneName)
    {
        FindObjectOfType<FadeTransitionManager>().StartFadeOut();
        while (!FindObjectOfType<FadeTransitionManager>().hasTransitionCompleted())
            yield return null;
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator gameplay ()
    {
        yield return new WaitForSeconds(2);
        for (int speed = 200; speed >= 50; speed -= 25)
            for (int wall = 0; wall < 2; wall++)
            {
                transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 0;
                walls[wall].SetActive(true);
                for (int i = 1; i <= Mathf.Max(100, speed); i++)
                {
                    walls[wall].transform.localScale = new Vector3(91, 51, 1) * (50 + i / (Mathf.Max(100, speed) / 50f)) * .01f;
                    wallApproaches = Mathf.Max(100, speed) - i;
                    if (speed == 50)
                        wallApproaches = 78;
                    walls[wall].GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, .5f, Mathf.Min(5, wallApproaches) * .2f);
                    walls[wall].transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.Min(5, wallApproaches) * .2f);
                    yield return new WaitForSeconds(.025f);
                }
                if (transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder == 0)
                {
                    walls[wall].GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, .5f, 1);
                    walls[wall].transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                    wallApproaches = 40;
                    StartCoroutine(loadScene("MGHoleInWall"));
                    yield return new WaitForSeconds(5);
                }
                if (speed == 50)
                {
                    GameState.minigamesCompleted++;
                    StartCoroutine(loadScene("TileRunner"));
                    yield return new WaitForSeconds(5);
                }
                walls[wall].transform.GetChild(0).localPosition = new Vector3(Random.Range(-.4f, .4f), Random.Range(-2, 3) * .2f);
                walls[wall].SetActive(false);
                foreach (StickMovement stick in FindObjectsOfType<StickMovement>())
                    stick.ramdomizePosition();
            }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.name == "FloorBounds")
        {
            GetComponent<Animator>().SetBool("Jumping", false);
            onFloor = true;
        }
    }
}
