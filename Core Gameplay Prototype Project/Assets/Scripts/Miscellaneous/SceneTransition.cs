using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SceneTransition : MonoBehaviour
{
    // Start is called before the first frame update
    public Image image;

    public float fadeDuration = 3;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // IEnumerator FadeIn(){
    //     image.gameObject.SetActive(false);
    //     image.CrossFadeAlpha(0,fadeDuration, false);
    //     yield return new WaitForSeconds(fadeDuration);
    // }
}
