using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextCrawlBehaviour : MonoBehaviour
{
    bool crawlOver = false;

    const float doubleTapWindow = 0.2f;
    float lastTapTime = 0f;

    AsyncOperation mainMenuLoadOp = null;

    void Start()
    {
        string targetScene = "MainMenu";

        if (!PlayerPrefs.HasKey("TutorialComplete") || PlayerPrefs.GetInt("TutorialComplete") == 0)
        {
            GameManager.Instance.tutorialMode = true;
            targetScene = "MapSelection";
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

        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastTapTime <= doubleTapWindow)
            {
                Time.timeScale = 1.0f;
                mainMenuLoadOp.allowSceneActivation = true;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            lastTapTime = Time.time;
        }
    }

    public void animationEvent(){
        Time.timeScale = 1.0f;
        crawlOver = true;
        mainMenuLoadOp.allowSceneActivation = true;
    }
}
