using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BombDetectable : MonoBehaviour, IBulletHittable 
{
    [SerializeField] GameObject Explosion;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnBulletHit(){
        Explosion.SetActive(true);   
    }

    private void OnDrawGizmos() {
        Gizmos.DrawSphere(transform.position, 2);
    }
}
