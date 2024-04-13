using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BombBehaviour : MonoBehaviour
{
    TargetType targetType;
    [SerializeField] float explosionRadius = 5f;
    InfestedBossBehaviour bossBehaviour;

    private bool spawnFromRight;
    // Start is called before the first frame update
    void Start()
    {
        bossBehaviour = FindFirstObjectByType<InfestedBossBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(explosionRadius > 0){
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (Collider2D hitCollider in hitColliders){
                if(collision.tag == "Enemy"){
                   collision.gameObject.SetActive(false);
                   bossBehaviour.enableCollider();
                }
            }
            
            
            Destroy(gameObject.transform.parent.gameObject);    
        }

        
    }

    
}
