using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject teamLogo;

    public GameObject textCrawl;
    private CanvasGroup image; 
    void Start()
    {
        image = GetComponent<CanvasGroup>();
        textCrawl.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            teamLogo.SetActive(false);
            textCrawl.SetActive(true);
        }
        else if(image.alpha == 0){
            teamLogo.SetActive(false);
            textCrawl.SetActive(true);
        }    
    }
}
