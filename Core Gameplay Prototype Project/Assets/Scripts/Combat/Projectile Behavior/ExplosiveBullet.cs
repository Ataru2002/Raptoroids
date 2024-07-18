using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBullet : Bullet
{
    [SerializeField] float armTime;
    float timeSinceSpawn = 0;

    [Tooltip("Time since spawning to explosion. Explodes as soon as armed if explode time is less than or equal to arm time.")]
    [SerializeField] float explodeTime;
    [SerializeField] float explodeRadius = 1;

    [SerializeField] int explosionMaxDamage;

    // TODO: Look into pooling the effect object if performance is poor
    [SerializeField] GameObject explosionFXPrefab;

    void OnEnable()
    {
        timeSinceSpawn = 0;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        timeSinceSpawn += Time.deltaTime;

        spriteRenderer.material.SetFloat("_TimeRatio", timeSinceSpawn / explodeTime);

        if (timeSinceSpawn >= armTime && timeSinceSpawn >= explodeTime)
        {
            Explode();
            despawn(gameObject);
        }
    }

    new void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == targetType.ToString() && timeSinceSpawn >= armTime)
        {
            Explode();
        }

        base.OnTriggerEnter2D(collision);
    }

    void Explode()
    {
        Collider2D[] explosionHits = Physics2D.OverlapCircleAll(transform.position, explodeRadius);

        foreach (Collider2D explosionHit in explosionHits)
        {
            IBulletHittable damageable = explosionHit.GetComponent<IBulletHittable>();
            if (damageable != null)
            {
                // Make sure to take the closest point to the center of the explosion, as the center of the 
                // hit object may be outside of the radius
                float distance = Vector2.Distance(transform.position, explosionHit.ClosestPoint(transform.position));
                float multiplier = 1 - (distance / explodeRadius);
                int finalDamage = Mathf.Clamp(Mathf.RoundToInt(multiplier * explosionMaxDamage), 1, explosionMaxDamage);

                damageable.OnBulletHit(finalDamage);
            }
        }

        if (explosionFXPrefab != null)
        {
            GameObject effectInstance = Instantiate(explosionFXPrefab, transform.position, Quaternion.identity);

            ParticleSystem explosionParticles = effectInstance.GetComponent<ParticleSystem>();
            var particleSpawnShape = explosionParticles.shape;
            particleSpawnShape.radius = explodeRadius;
            explosionParticles.Emit(250);


            AudioSource audioFX = effectInstance.GetComponent<AudioSource>();
            audioFX.PlayOneShot(audioFX.clip);

            Destroy(effectInstance, audioFX.clip.length);
        }
    }

    new public void OnGetFromPool()
    {
        base.OnGetFromPool();
        timeSinceSpawn = 0;
    }
}
