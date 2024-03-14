using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialNavigator : MonoBehaviour
{
    [SerializeField] GameObject combatCanvas;
    [SerializeField] GameObject mapCanvas;
    [SerializeField] GameObject instructionCanvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
                combatCanvas.SetActive(true);
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
            case "combat":
                combatCanvas.SetActive(false);
                break;
            default:
                break;
        }
    }
}
