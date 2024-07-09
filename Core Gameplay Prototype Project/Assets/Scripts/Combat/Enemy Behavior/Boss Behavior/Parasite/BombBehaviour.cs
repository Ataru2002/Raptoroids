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

    private void Awake()
    {
        explosionFXSource = GetComponent<AudioSource>();
        explosionParticles = explosionVisual.GetComponent<ParticleSystem>();
    }

    public void Detonate()
    {
        explosionVisual.SetActive(true);
        explosionFXSource.PlayOneShot(explosionFXSource.clip);
        explosionParticles.Emit(80);
        transform.parent.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        transform.parent.GetComponent<SpriteRenderer>().enabled = false;
        StartCoroutine(bombVisualDelay(0.6f));

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

    IEnumerator bombVisualDelay(float duration){
        yield return new WaitForSeconds(duration);
        Destroy(transform.parent.gameObject);
    }
}
