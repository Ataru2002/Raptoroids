using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfestedRaptoroidHittable : MonoBehaviour, IBulletHittable
{
    int pointsAwarded = 20;
    [SerializeField] EnemyHealth enemyHealth;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBulletHit(){
        enemyHealth.TakeDamage(4);
        CombatStageManager.Instance.UpdateScore(pointsAwarded);
    }
}
