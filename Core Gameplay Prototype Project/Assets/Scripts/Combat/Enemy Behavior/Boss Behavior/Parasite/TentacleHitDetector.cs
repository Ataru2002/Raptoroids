using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleHitDetector : MonoBehaviour, IBulletHittable
{
    // Start is called before the first frame update
    TentacleBehavior tentacleBehavior;
    void Start()
    {
        
    }

    void Awake(){
        tentacleBehavior = GetComponentInParent<TentacleBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBulletHit(int damage = 1){
        tentacleBehavior.NotifytentacleHit(damage);
    }
}
