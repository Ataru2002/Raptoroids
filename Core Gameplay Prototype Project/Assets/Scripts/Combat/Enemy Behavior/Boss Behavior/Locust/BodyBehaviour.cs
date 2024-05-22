using System;
using System.Collections;
using UnityEngine;

public class BodyBehaviour : MonoBehaviour
{   
    [SerializeField] GameObject hitBox;
    [SerializeField] EnemyHealth combinedBossHP;
    StrafeEnemyBehavior strafeBehaviour;
    FadingEffect fadingEffect;
    Action stateUpdate = null;
    float attackMoveSpeed = 12f;
    int bodyHP = 15;
    int pointsAwarded = 20;
    Transform playerCurrentPos;
    bool attackStarted = false;
    bool fadeInDone = true;
    PlayerHealth playerHealth;
    Color opacity;
    // Start is called before the first frame update

    void Awake(){
        fadingEffect = GetComponent<FadingEffect>();
        opacity = GetComponent<SpriteRenderer>().color;
        strafeBehaviour = GetComponent<StrafeEnemyBehavior>();
        
    }
    void Start()
    {
        hitBox.SetActive(true);
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
        
        UpdatePlayerPos();
        stateUpdate = Attack;
        while(Vector3.Distance(transform.position, playerCurrentPos.position) > 0.2f){
            yield return new WaitForEndOfFrame();
        }
        
        UpdatePlayerPos();
        // strafeBehaviour.publicInvertStrafe = playerCurrentPos.position.x <= transform.position.x ? true : false;
        
        stateUpdate = DoNothing;
        
        fadingEffect.tryFadeIn();
        // strafeBehaviour.enabled = true;
        yield return new WaitForSeconds(1.5f);
        // strafeBehaviour.enabled = false;
        // strafeBehaviour.publicInvertStrafe = false;
        stateUpdate = DoNothing;

        // strafeBehaviour.enabled = false;
        attackStarted = false;
    }   

    void Attack()
    {
        transform.position = Vector3.MoveTowards(transform.position, playerCurrentPos.position, attackMoveSpeed * Time.deltaTime);
    }

    void UpdatePlayerPos(){
        playerCurrentPos = CombatStageManager.Instance.PlayerTransform;

    }

    void Strafe(){

    }
    void DoNothing(){
        return;
    }
    // void TrackPlayer(){
    //     LookAtTarget(CombatStageManager.Instance.PlayerTransform.position);
    // }
//     void LookAtTarget(Vector3 target)
//     {
//         attackDirection = target - transform.position;
//         float angle = Mathf.Atan2(attackDirection.y, attackDirection.x);
//         angle += Mathf.PI / 2f;
//         angle *= Mathf.Rad2Deg;
//         transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
//     }
}
