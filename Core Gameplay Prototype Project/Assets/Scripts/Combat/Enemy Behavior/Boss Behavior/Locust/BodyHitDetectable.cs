using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyHitDetectable : MonoBehaviour, IBulletHittable
{
    MantisBodyBehaviour bodyBehaviour;
    // Start is called before the first frame update
    void Start()
    {
        bodyBehaviour = GetComponentInParent<MantisBodyBehaviour>();
    }

    public void OnBulletHit(int damage = 1)
    {
        bodyBehaviour.NotifyBodyHit();
    }
}
