using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonListener : MonoBehaviour
{
    private GameObject[] panels;
    private GameObject defaultMenu;

    [SerializeField] GameObject creditsScreen;

    // Start is called before the first frame update
    void Start()
    {
        // settingsPanel = GameObject.FindGameObjectWithTag("Panel");
        panels = GameObject.FindGameObjectsWithTag("Panel");
        foreach(GameObject panel in panels){
            panel.SetActive(false);
        }
        defaultMenu = GameObject.FindGameObjectWithTag("DefaultMenu"); 
    }

    public void playListener(){
        StartCoroutine(loadScene("MapSelection"));
        ButtonSFXPlayer.Instance.PlaySFX("ToMaps");
    }

    public void shopListener(){
        foreach(GameObject panel in panels){
            if(panel.name == "CanvasShop"){
                panel.SetActive(true);
            }
        }
        defaultMenu.SetActive(false);
        ButtonSFXPlayer.Instance.PlaySFX("MenuMove");
    }

    public void loadoutListener(){
        foreach(GameObject panel in panels){
            if(panel.name == "CanvasLoadout"){
                panel.SetActive(true);
            }
        }

        defaultMenu.SetActive(false);
        ButtonSFXPlayer.Instance.PlaySFX("MenuMove");
    }

    public void settingsListener(){
        
        foreach(GameObject panel in panels){
            if(panel.name == "CanvasSettings"){
                panel.SetActive(true);
            }
        }
        
        defaultMenu.SetActive(false);
        ButtonSFXPlayer.Instance.PlaySFX("MenuMove");
    }

    public void backListener(){
        foreach(GameObject panel in panels){
            panel.SetActive(false);
        }
    
        defaultMenu.SetActive(true);
        ButtonSFXPlayer.Instance.PlaySFX("MenuMove");
    }

    public void TutorialListener()
    {
        GameManager.Instance.StartTutorial();
        StartCoroutine(loadScene("MapSelection"));
    }

    public void QuestListener()
    {
        StartCoroutine(loadScene("Quests"));
        ButtonSFXPlayer.Instance.PlaySFX("MenuMove");
    }

    public void CreditsListener()
    {
        creditsScreen.SetActive(true);
        defaultMenu.SetActive(false);
        ButtonSFXPlayer.Instance.PlaySFX("MenuMove");
    }

    IEnumerator loadScene(string sceneName){
        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(sceneName);

        while(!sceneLoad.isDone){   //in the future change this
            yield return null;
        }
    }
}
