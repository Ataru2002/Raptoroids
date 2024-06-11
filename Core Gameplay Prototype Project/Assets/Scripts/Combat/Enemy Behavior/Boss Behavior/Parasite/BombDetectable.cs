using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDetectable : MonoBehaviour, IBulletHittable 
{
    [SerializeField] BombBehaviour bomb;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void OnBulletHit(int damage = 1){
        bomb.Detonate();
    }

    // private void OnDrawGizmos() {
    //     Gizmos.DrawSphere(transform.position, 2);
    // }
}
