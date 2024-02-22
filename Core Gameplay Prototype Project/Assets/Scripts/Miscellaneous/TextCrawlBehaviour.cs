using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextCrawlBehaviour : MonoBehaviour
{
    // Start is called before the first frame update

    public int textOutOfScreen = 900;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            SceneManager.LoadScene("MainMenu");
        }
           
    }

    public void animationEvent(){
        SceneManager.LoadScene("MainMenu");
    }
}
