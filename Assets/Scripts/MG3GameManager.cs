using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MG3GameManager : MonoBehaviour
{
    public GameObject dog;
    bool dogColliding = false;
    public static bool startGame = false;
    public AudioClip[] clips;
    bool petting = false;
    Rigidbody2D rb;
    int petNumber = 0;
    bool facing = true;
    bool transitioning = false;

    // Start is called before the first frame update
    void Start()
    {
        startGame = false;
        FindObjectOfType<FadeTransitionManager>().StartFadeIn();
        rb = transform.GetComponent<Rigidbody2D>();
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
        if (!petting)
        {
            if (Input.GetKey(KeyCode.A))
            {
                rbVelocity -= Vector2.right * 25;
                dogColliding = false;
                facing = true;
            }
            if (Input.GetKey(KeyCode.D) && !dogColliding)
            {
                rbVelocity += Vector2.right * 25;
                facing = false;
            }
            if (Input.GetKeyDown(KeyCode.Space) && startGame)
                StartCoroutine(pet());
        }
        GetComponent<Animator>().SetBool("Moving", rb.velocity.x != 0);
        GetComponent<Animator>().SetBool("Petting", petNumber > 0);
        if (rb.velocity.x != 0)
            transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = facing;
        rb.velocity = new Vector2((rbVelocity.x + rb.velocity.x) / 2, rbVelocity.y);
    }

    IEnumerator loadScene(string sceneName)
    {
        if (!transitioning)
        {
            if (sceneName == "MGDogAttack")
                GetComponent<AudioSource>().clip = clips[0];
            else
                GetComponent<AudioSource>().clip = clips[1];
            GetComponent<AudioSource>().Play();
            transitioning = true;
            FindObjectOfType<FadeTransitionManager>().StartFadeOut();
            while (!FindObjectOfType<FadeTransitionManager>().hasTransitionCompleted())
                yield return null;
            SceneManager.LoadScene(sceneName);
        }
    }

    IEnumerator gameplay ()
    {
        StartCoroutine(timer());
        foreach (DogPhysics dog in FindObjectsOfType<DogPhysics>())
            StartCoroutine(dog.dogRandomness());
        for (int i = 0; i < 35; i++)
        {
            Instantiate(dog, new Vector3(70, 5), Quaternion.Euler(0, 0, 90));
            yield return new WaitForSeconds(.75f);
        }
    }

    IEnumerator timer ()
    {
        Transform timingBar = GameObject.Find("TimerDeduction").transform;
        Transform dogCount = GameObject.Find("DogAmount").transform;
        for (float i = 60; i > 0; i -= Time.deltaTime * 1)
        {
            timingBar.localPosition = Vector3.right * (i / 60f);
            int dogsTouching = 0;
            foreach (DogPhysics dog in FindObjectsOfType<DogPhysics>())
                if (dog.touching > 0)
                    dogsTouching++;
            dogCount.GetComponent<TextMeshPro>().text = "Dogs in Contact: " + dogsTouching + " (< 3)";
            if (dogsTouching > 0)
                rb.velocity -= Vector2.right * Mathf.Max(0, transform.position.x) * .25f;
            if (dogsTouching > 2)
                break;
            yield return null;
        }
        StartCoroutine(loadScene("MGDogAttack"));
    }

    IEnumerator pet ()
    {
        DogPhysics closestDog = FindObjectOfType<DogPhysics>();
        foreach (DogPhysics dog in FindObjectsOfType<DogPhysics>())
            if (dog.gameObject.layer != 8 && Vector3.Distance(closestDog.transform.position, transform.position) > Vector3.Distance(dog.transform.position, transform.position))
                closestDog = dog;
        if (Vector3.Distance(closestDog.transform.position, transform.position) < 13)
        {
            petting = true;
            closestDog.pet++;
            petting = false;
            if (closestDog.pet == 7)
                dogColliding = false;
            petNumber++;
            closestDog.displayHearts();
            yield return new WaitForSeconds(1);
            petNumber--;
        }
        yield return null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.name == "RightBounds")
            StartCoroutine(leaveGame());
    }

    IEnumerator leaveGame ()
    {
        GameState.minigamesCompleted++;
        GetComponent<Rigidbody2D>().isKinematic = false;
        GetComponent<BoxCollider2D>().enabled = false;
        for (int i = 0; i < 100; i++)
        {
            transform.position += Vector3.right * Time.deltaTime * 5;
            yield return null;
        }
        StartCoroutine(loadScene("TileRunner"));
    }
}
