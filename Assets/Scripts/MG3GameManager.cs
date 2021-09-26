using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MG3GameManager : MonoBehaviour
{
    public GameObject dog;
    bool dogColliding = false;
    public static bool startGame = false;
    bool petting = false;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
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
            }
            if (Input.GetKey(KeyCode.D) && !dogColliding)
                rbVelocity += Vector2.right * 25;
            if (Input.GetKeyDown(KeyCode.Space) && startGame)
                StartCoroutine(pet());
        }
        //rbVelocity.x -= Mathf.Max(0, transform.position.x) * .25f;
        rb.velocity = new Vector2((rbVelocity.x + rb.velocity.x) / 2, rbVelocity.y);
    }

    IEnumerator gameplay ()
    {
        StartCoroutine(timer());
        foreach (DogPhysics dog in FindObjectsOfType<DogPhysics>())
            StartCoroutine(dog.dogRandomness());
        for (int i = 0; i < 35; i++)
        {
            Instantiate(dog, new Vector3(70, 5), Quaternion.Euler(0, 0, 90));
            yield return new WaitForSeconds(1.5f);
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
            dogCount.GetComponent<TextMeshPro>().text = "Dogs in Contact: " + dogsTouching;
            yield return null;
        }
    }

    IEnumerator pet ()
    {
        DogPhysics closestDog = FindObjectOfType<DogPhysics>();
        foreach (DogPhysics dog in FindObjectsOfType<DogPhysics>())
            if (dog.gameObject.layer != 8 && Vector3.Distance(closestDog.transform.position, transform.position) > Vector3.Distance(dog.transform.position, transform.position))
                closestDog = dog;
        if (Vector3.Distance(closestDog.transform.position, transform.position) < 15)
        {
            petting = true;
            closestDog.pet++;
            petting = false;
            if (closestDog.pet == 7)
                dogColliding = false;
        }
        yield return null;
    }
}
