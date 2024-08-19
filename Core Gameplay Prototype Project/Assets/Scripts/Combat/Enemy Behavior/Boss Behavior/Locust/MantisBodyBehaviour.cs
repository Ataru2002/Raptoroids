using System;
using System.Collections;
using UnityEngine;

public class MantisBodyBehaviour : MonoBehaviour
{   
    [SerializeField] GameObject hitBox;
    [SerializeField] EnemyHealth combinedBossHP;
    FadingEffect fadingEffect;
    StatusEffectHandler statusEffectHandler;

    Action stateUpdate = null;
    float attackMoveSpeed = 12f;
    int bodyHP = 15;
    int pointsAwarded = 20;
    Transform playerTransform;
    bool attackStarted = false;
    bool fadeInDone = true;
    PlayerHealth playerHealth;

    Color opacity;
    // Start is called before the first frame update

    void Awake()
    {
        fadingEffect = GetComponent<FadingEffect>();
        opacity = GetComponent<SpriteRenderer>().color;
        statusEffectHandler = GetComponent<StatusEffectHandler>();
    }

    void Start()
    {
        hitBox.SetActive(true);
        playerTransform = CombatStageManager.Instance.PlayerTransform;
    }

    // Update is called once per frame
    void Update()
    {
        if(stateUpdate != null){
            stateUpdate();
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Player"){
            playerHealth = other.GetComponent<PlayerHealth>();
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Player"){
            playerHealth = null;
        }
    }

    public void NotifyBodyHit(){
        if(fadingEffect.fadedIn){
            int damage = 1;
            bodyHP -= damage;
            CombatStageManager.Instance.UpdateScore(20);
            combinedBossHP.TakeDamage(damage);

            if(bodyHP <= 0){
                StopAllCoroutines();
                Destroy(gameObject);
            }
        }
        
    }

    public void TryAttack(){
        if(playerHealth != null){
            playerHealth.OnBulletHit();
        }
    }

    public void TryInvisAttack(){
        if(attackStarted){
            return;
        }

        StartCoroutine(invisAttackSequence());
    }

    IEnumerator invisAttackSequence(){
        attackStarted = true;
        fadingEffect.tryFadeOut();
        yield return new WaitForSeconds(1f);
        
        stateUpdate = Stalk;
        while(Vector3.Distance(transform.position, playerTransform.position) > 0.2f){
            yield return new WaitForEndOfFrame();
        }
        
        stateUpdate = null;
        
        fadingEffect.tryFadeIn(true);

        yield return new WaitForSeconds(1.5f);
        yield return new WaitUntil(() => !statusEffectHandler.HasStatusCondition(StatusEffect.Stun));

        attackStarted = false;
    }   

    void Stalk()
    {
        transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, attackMoveSpeed * Time.deltaTime);
    }
}
