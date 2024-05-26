using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class GemVeilControl : MonoBehaviour
{
    [SerializeField] LocalizeStringEvent tutorialMessageLocalizeEvent;
    [SerializeField] GameObject tutorialMessageBox;
    int clickCount = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Advance();
        }
    }

    void Advance()
    {
        if (++clickCount == 1)
        {
            tutorialMessageLocalizeEvent.SetEntry("GemCollected2");
            tutorialMessageLocalizeEvent.RefreshString();
        }
        else
        {
            Time.timeScale = 1.0f;
            tutorialMessageBox.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
