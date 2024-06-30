using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchthroughBullet : Bullet
{
    new void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlayerBullet)
        {
            GameObject particles;
            if (CombatStageManager.Instance != null)
            {
                particles = CombatStageManager.Instance.GetEnemyHitParticles();
            }
            else
            {
                particles = TutorialRoomManager.Instance.GetEnemyHitParticles();
            }
            particles.transform.position = transform.position;
            particles.GetComponent<ParticleSystem>().Emit(10);
        }

        IBulletHittable bulletHitDetector = collision.GetComponent<IBulletHittable>();
        if (bulletHitDetector != null)
        {
            bulletHitDetector.OnBulletHit(damage);
        }
    }
}
