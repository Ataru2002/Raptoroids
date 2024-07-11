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

    public void OnBulletHit(int damage = 1)
    {
        pincerBehavior.NotifyPincerHit(damage);
    }
}
