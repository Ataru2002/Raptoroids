using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class BombBehaviour : MonoBehaviour
{
    [SerializeField] float explosionRadius = 2f;
    InfestedBossBehaviour bossBehaviour;
    [SerializeField] GameObject explosionVisual;
    
    [SerializeField] ParasiteManager shieldStatusUpdate;
    // Start is called before the first frame update
    void Start()
    {
        bossBehaviour = FindFirstObjectByType<InfestedBossBehaviour>();
        shieldStatusUpdate = GetComponent<ParasiteManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(explosionRadius > 0){
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (Collider2D hitCollider in hitColliders){
                if(collision.tag == "Enemy") {
                    // Make sure the bomb affects only the shield
                    InfestedRaptoroidShield shieldComponent = collision.GetComponent<InfestedRaptoroidShield>();
                    if (shieldComponent == null)
                    {
                        continue;
                    }

                    explosionVisual.transform.localScale = new Vector3(explosionRadius, explosionRadius, 1); 
                    collision.gameObject.SetActive(false);
                    disableBombCollider();
                    
                    ParasiteManager.Instance.SetShieldStatus(false);
                    break;
                }
            }
            StartCoroutine(bombVisualDelay(0.3f));
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
