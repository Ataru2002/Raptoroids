using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class BombBehaviour : MonoBehaviour
{
    [SerializeField] float explosionRadius = 2f;
    [SerializeField] GameObject explosionVisual;
    AudioSource explosionFXSource;
    ParticleSystem explosionParticles;

    Coroutine selfDestruct;

    private void Awake()
    {
        explosionFXSource = GetComponent<AudioSource>();
        explosionParticles = explosionVisual.GetComponent<ParticleSystem>();
    }

    void Start()
    {
        selfDestruct = StartCoroutine(DestroyOutOfRange());
    }

    public void Detonate()
    {
        explosionVisual.SetActive(true);
        explosionFXSource.PlayOneShot(explosionFXSource.clip);
        explosionParticles.Emit(80);
        transform.parent.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        transform.parent.GetComponent<SpriteRenderer>().enabled = false;
        StartCoroutine(BombVisualDelay(0.6f));

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.tag == "Enemy")
            {
                // Make sure the bomb affects only the shield
                InfestedRaptoroidShield shieldComponent = hitCollider.GetComponent<InfestedRaptoroidShield>();
                if (shieldComponent == null)
                {
                    continue;
                }

                explosionVisual.transform.localScale = new Vector3(explosionRadius, explosionRadius, 1);
                hitCollider.gameObject.SetActive(false);

                ParasiteManager.Instance.SetShieldStatus(false);
                break;
            }
        }
    }

    IEnumerator BombVisualDelay(float duration){
        yield return new WaitForSeconds(duration);
        StopCoroutine(selfDestruct);
        Destroy(transform.parent.gameObject);
    }

    IEnumerator DestroyOutOfRange()
    {
        Rigidbody2D rb = GetComponentInParent<Rigidbody2D>();
        yield return null;
        if (rb.velocityX < 0)
        {
            while (transform.parent.gameObject.transform.position.x > CombatStageManager.Instance.HorizontalLowerBound)
            {
                yield return null;
            }
        }
        else
        {
            while (transform.parent.gameObject.transform.position.x < CombatStageManager.Instance.HorizontalUpperBound)
            {
                yield return null;
            }
        }

        Destroy(transform.parent.gameObject);
    }
}
