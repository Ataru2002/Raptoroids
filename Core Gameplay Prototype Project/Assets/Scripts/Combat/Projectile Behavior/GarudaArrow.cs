using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarudaArrow : Bullet
{
    const float stunTime = 3.0f;

    new void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == targetType.ToString())
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

            StatusEffectHandler targetStatusHandler = collision.GetComponentInParent<StatusEffectHandler>();
            if (targetStatusHandler != null)
            {
                targetStatusHandler.SetStatusCondition(StatusEffect.Stun, stunTime);
            }

            IBulletHittable bulletHitDetector = collision.GetComponent<IBulletHittable>();
            if (bulletHitDetector != null)
            {
                bulletHitDetector.OnBulletHit(damage);
            }
        }
    }
}
