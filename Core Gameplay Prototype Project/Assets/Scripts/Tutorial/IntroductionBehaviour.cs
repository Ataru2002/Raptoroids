using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroductionBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public static event Action onIntroduction;


    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.tag == "Player"){
            print("collision working");
            onIntroduction?.Invoke();
            Destroy(gameObject);
        }
        
    }
}
