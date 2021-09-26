using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TheEndManager : MonoBehaviour
{
    public String mainMenuSceneName = "MainMenu";
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMainMenuButton()
    {
        GameState.obstaclesHits = 0;
        GameState.minigamesCompleted = 0;

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
