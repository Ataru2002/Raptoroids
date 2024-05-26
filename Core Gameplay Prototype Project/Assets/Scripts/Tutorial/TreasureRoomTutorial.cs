using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureRoomTutorial : MonoBehaviour
{
    Coroutine autoHide;
    [SerializeField] GameObject timerObject;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance.tutorialMode && !GameManager.Instance.tutorialTreasureRoomMessageDisplayed)
        {
            timerObject.SetActive(false);
            Time.timeScale = 0;
            GameManager.Instance.tutorialTreasureRoomMessageDisplayed = true;
        }

        else
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Hide();
        }
    }

    void Hide()
    {
        Time.timeScale = 1;
        timerObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
