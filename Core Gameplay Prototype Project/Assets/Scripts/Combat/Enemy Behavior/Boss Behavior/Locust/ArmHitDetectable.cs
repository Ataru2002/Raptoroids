using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmHitDetectable : MonoBehaviour, IBulletHittable
{
    // Start is called before the first frame update
    GiantArmBehaviour giantArm;
    void Start()
    {
        giantArm = GetComponentInParent<GiantArmBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnBulletHit(int damage = 1){
        giantArm.NotifyArmHit(damage);
    }
}
