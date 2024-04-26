using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDetectable : MonoBehaviour, IBulletHittable 
{
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject explosionVisual;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void OnBulletHit(){

        explosion.SetActive(true);
        explosionVisual.SetActive(true);
    }

    // private void OnDrawGizmos() {
    //     Gizmos.DrawSphere(transform.position, 2);
    // }
}
