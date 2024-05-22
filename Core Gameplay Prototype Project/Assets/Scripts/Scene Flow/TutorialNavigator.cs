using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialNavigator : MonoBehaviour
{
    [SerializeField] GameObject mapCanvas;
    [SerializeField] GameObject instructionCanvas;

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SelectSubmenu(string submenu)
    {
        instructionCanvas.SetActive(false);

        switch(submenu)
        {
            case "map":
                mapCanvas.SetActive(true);
                break;
            case "combat":
                SceneManager.LoadScene("Tutorial");
                break;
            default:
                break;
        }
    }

    public void ReturnToTutorialMain(string submenu)
    {
        instructionCanvas.SetActive(true);

        switch (submenu)
        {
            case "map":
                mapCanvas.SetActive(false);
                break;
            default:
                break;
        }
    }
}
