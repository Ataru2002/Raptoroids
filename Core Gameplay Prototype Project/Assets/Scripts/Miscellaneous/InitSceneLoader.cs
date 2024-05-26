using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitSceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Testing purposes: force start at tutorial
        //PlayerPrefs.SetInt("TutorialComplete", 0);
        SceneManager.LoadScene(1);
    }
}
