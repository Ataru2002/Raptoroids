using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedePincerHitDetector : MonoBehaviour, IBulletHittable
{
    CentipedePincerBehavior pincerBehavior;

    void Awake()
    {
        pincerBehavior = GetComponentInParent<CentipedePincerBehavior>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBulletHit(int damage = 1)
    {
        pincerBehavior.NotifyPincerHit(damage);
    }
}
