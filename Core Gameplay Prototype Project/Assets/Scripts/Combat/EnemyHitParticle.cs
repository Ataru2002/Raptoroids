using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitParticle : MonoBehaviour
{
    ParticleSystem particles;

    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!particles.IsAlive())
        {
            CombatStageManager.Instance.ReturnEnemyParticles(gameObject);
        }
    }
}
