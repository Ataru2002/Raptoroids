using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerShop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void backListener(){
        StartCoroutine(loadScene("MainMenu"));
    }

    IEnumerator loadScene(string sceneName){
        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(sceneName);

        while(!sceneLoad.isDone){   //in the future change this
            yield return null;
        }
    }  
}
