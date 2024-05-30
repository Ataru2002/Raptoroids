using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyHitDetectable : MonoBehaviour, IBulletHittable
{
    BodyBehaviour bodyBehaviour;
    // Start is called before the first frame update
    void Start()
    {
        bodyBehaviour = GetComponentInParent<BodyBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBulletHit(int damage = 1){
        bodyBehaviour.NotifyBodyHit();
    }
}
