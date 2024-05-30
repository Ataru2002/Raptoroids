using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeLegHitDetector : MonoBehaviour, IBulletHittable
{
    CentipedeLegBehavior legBehavior;

    void Awake()
    {
        legBehavior = GetComponentInParent<CentipedeLegBehavior>();
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
        legBehavior.NotifyLegHit(damage);
    }
}
