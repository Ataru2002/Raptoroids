using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class BombBehaviour : MonoBehaviour
{
    [SerializeField] float explosionRadius = 2f;
    [SerializeField] GameObject explosionVisual;

    public void Detonate()
    {
        explosionVisual.SetActive(true);
        StartCoroutine(bombVisualDelay(0.3f));

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
                disableBombCollider();

                ParasiteManager.Instance.SetShieldStatus(false);
                break;
            }
        }
    }

    void disableBombCollider(){
        BoxCollider2D parentCollider =  transform.parent.gameObject.GetComponent<BoxCollider2D>();
        if(parentCollider != null){
            parentCollider.enabled = false;
        }
        CircleCollider2D childCollider = GetComponent<CircleCollider2D>();
        childCollider.enabled = false;
    }

    IEnumerator bombVisualDelay(float duration){
        yield return new WaitForSeconds(duration);
        transform.parent.gameObject.SetActive(false);
    }
}
