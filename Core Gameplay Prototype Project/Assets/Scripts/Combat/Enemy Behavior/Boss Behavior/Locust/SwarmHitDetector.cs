using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmHitDetectable : MonoBehaviour, IBulletHittable
{
    SwarmBehaviour swarm; 
    // Start is called before the first frame update
    void Start()
    {
        swarm = GetComponentInParent<SwarmBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBulletHit(int damage = 1){
        swarm.NotifySwarmHit(damage);
    }

}
