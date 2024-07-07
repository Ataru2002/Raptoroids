using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemProjectile : MonoBehaviour
{
    public float speed;

    void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);

        if(transform.position.y < -5f){
            TreasureStageManager.Instance.ReturnDiamondProjectile(gameObject);
        }    
    }
}
