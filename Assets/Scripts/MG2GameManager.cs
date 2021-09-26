using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MG2GameManager : MonoBehaviour
{
    public static int wallApproaches = 50;
    public GameObject[] walls;
    public GameObject leafParticle;
    public AudioClip[] clips;
    bool startGame = false;
    Rigidbody2D rb;
    bool onFloor = true;

    // Start is called before the first frame update
    void Start()
    {
        wallApproaches = 50;
        FindObjectOfType<FadeTransitionManager>().StartFadeIn();
        rb = transform.GetComponent<Rigidbody2D>();
        walls[0].transform.GetChild(0).localPosition = new Vector3(Random.Range(-7f, 7f), Random.Range(-2, 3) * 1.75f);
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
        if (sceneName == "MGHoleInWall")
            GetComponent<AudioSource>().clip = clips[0];
        else
            GetComponent<AudioSource>().clip = clips[1];
        GetComponent<AudioSource>().Play();
        FindObjectOfType<FadeTransitionManager>().StartFadeOut();
        while (!FindObjectOfType<FadeTransitionManager>().hasTransitionCompleted())
            yield return null;
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator gameplay ()
    {
        Transform bkgd = GameObject.Find("ParentGameBackground").transform;
        yield return new WaitForSeconds(2);
        for (int speed = 200; speed >= 50; speed -= 25)
            for (int wall = 0; wall < 2; wall++)
            {
                transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 0;
                walls[wall].SetActive(true);
                if (speed != 200 || wall == 1)
                    walls[wall].GetComponentInChildren<Animator>().SetBool("NewJump", true);
                for (int i = 1; i <= Mathf.Max(100, speed); i++)
                {
                    bkgd.localScale += Vector3.one * .005f;
                    bkgd.transform.position += Vector3.up * .01f;
                    walls[2].transform.localScale = new Vector3(5, 5, 1) * (50 + i / (Mathf.Max(100, speed) / 50f)) * .005f;
                    walls[wall].transform.localScale = new Vector3(5, 5, 1) * (50 + i / (Mathf.Max(100, speed) / 50f)) * .01f;
                    wallApproaches = Mathf.Max(100, speed) - i;
                    if (speed == 50)
                        wallApproaches = 78;
                    walls[wall].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.Min(5, wallApproaches) * .2f);
                    walls[wall].transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.Min(5, wallApproaches) * .2f);
                    yield return new WaitForSeconds(.025f);
                }
                if (transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder == 0)
                {
                    walls[wall].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
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
                walls[wall].transform.GetChild(0).localPosition = new Vector3(Random.Range(-7f, 7f), Random.Range(-2, 3) * 1.75f);
                walls[wall].GetComponentInChildren<Animator>().SetBool("NewJump", false);
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

    public void showLeaves ()
    {
        if (walls[0].activeSelf)
            Instantiate(leafParticle, walls[0].transform.GetChild(0).position, Quaternion.identity);
        else
            Instantiate(leafParticle, walls[1].transform.GetChild(0).position, Quaternion.identity);
    }
}
