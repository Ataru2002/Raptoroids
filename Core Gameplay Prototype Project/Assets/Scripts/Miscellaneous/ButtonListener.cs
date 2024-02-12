using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonListener : MonoBehaviour
{
    private GameObject settingsPanel;
    private GameObject[] allOtherButtons; 
    // Start is called before the first frame update
    void Start()
    {
        settingsPanel = GameObject.FindGameObjectWithTag("Panel");
        settingsPanel.SetActive(false);
        allOtherButtons = GameObject.FindGameObjectsWithTag("otherButton"); 
        Debug.Log(settingsPanel);
    }

    // Update is called once per frame
 

    public void playListener(){
        StartCoroutine(loadScene("MapSelection"));
    }

    public void shopListener(){
        StartCoroutine(loadScene("Shop"));
    }

    public void loadoutListener(){
        StartCoroutine(loadScene("Loadout"));
    }

    public void settingsListener(){
        settingsPanel.SetActive(true);
        foreach (GameObject button in allOtherButtons){
            button.SetActive(false);
        }
    }

    public void backListener(){
        settingsPanel.SetActive(false);
        foreach (GameObject button in allOtherButtons){
            button.SetActive(true);
        }
    }

    IEnumerator loadScene(string sceneName){
        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(sceneName);

        while(!sceneLoad.isDone){   //in the future change this
            yield return null;
        }
    }
}
