using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextCrawlBehaviour : MonoBehaviour
{
    bool crawlOver = false;

    AsyncOperation mainMenuLoadOp = null;

    void Start()
    {
        string targetScene = "MainMenu";

        if (!PlayerPrefs.HasKey("TutorialComplete") || PlayerPrefs.GetInt("TutorialComplete") == 0)
        {
            targetScene = "Tutorial";
        }

        mainMenuLoadOp = SceneManager.LoadSceneAsync(targetScene);
        mainMenuLoadOp.allowSceneActivation = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (crawlOver)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Time.timeScale = 2.5f;
        }
        
        else if (Input.GetMouseButtonUp(0))
        {
            Time.timeScale = 1.0f;
        }
    }

    public void animationEvent(){
        Time.timeScale = 1.0f;
        crawlOver = true;
        mainMenuLoadOp.allowSceneActivation = true;
    }
}
