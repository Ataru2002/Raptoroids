using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    #region Public Variables (attack, speed, delay)
    public float attackMoveSpeed = 6f;
    public float speed = 4f;
    public float minAttackDelay = 3f; 
    public float maxAttackDelay = 5f;
    #endregion 
    #region Booleans
    bool isVulnerable = false;
    bool swarmSequenceStarted = false;
    bool alreadyHitPlayer = false;
    bool swarmDestroyed = false;
    bool fusionSequenceStarted = false;
    #endregion
    #region Specific to this script and misc (Health, travel line, action, Points, spriteRender)
    BezierCurve returnLine = null;
    Action stateUpdate = null;
    SpriteRenderer spriteRenderer;
    StatusEffectHandler statusEffectHandler;
    EnemyHealth combinedSwarmHP;
    LocustAttackSequence controller;
    BoxCollider2D boxCollider;
    int swarmHP = 5;
    int pointsAwarded = 25;
    float timeSinceReturnStart = 0;
    Vector3 fusionLocation = new Vector3(0, 2, 0);
    const float returnTime = 2f;
    private float swarmDirection = 1f;
    private Vector3 originalPosition;
    private Coroutine flickerCoroutine;
    private Coroutine attackCoroutine;
    #endregion
    public static event Action onFusionComplete; 
    
    
    void Awake()
    {
        combinedSwarmHP = GetComponentInParent<EnemyHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        controller = GetComponentInParent<LocustAttackSequence>();
        statusEffectHandler = GetComponent<StatusEffectHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if(stateUpdate != null){
            stateUpdate();
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        if(!alreadyHitPlayer && other.tag == "Player"){
            other.GetComponent<IBulletHittable>().OnBulletHit();
            alreadyHitPlayer = true;
        }
    }
    
    #region "public funcs (communicate with other scripts)"

    public void NotifySwarmHit(int damage){
        
        if(swarmDestroyed){
            return;
        }

        if(isVulnerable){
            swarmHP -= damage;
            CombatStageManager.Instance.UpdateScore(pointsAwarded);
            combinedSwarmHP.TakeDamage(damage);

            if(swarmHP <= 0){
                swarmDestroyed = true;
                StopAllCoroutines();
                controller.miniEnemiesDestroyed += 1;
                Destroy(gameObject);
            }
        }
    }
    public void TrySwarmAttack(){
        if(swarmSequenceStarted || fusionSequenceStarted){
            return;
        }
        attackCoroutine = StartCoroutine(SwarmAttackSequence());
    }

    public void TryFusion(){
        if(fusionSequenceStarted){
            return;
        }
        StartCoroutine(FusionSequence());
    }
    #endregion
    
    #region "coroutines"

    IEnumerator SwarmAttackSequence(){
        isVulnerable = true;
        swarmSequenceStarted = true;
        originalPosition = transform.position;
        // float delay = UnityEngine.Random.Range(minAttackDelay, maxAttackDelay);
        
        stateUpdate = TrackPlayer;
        yield return new WaitForSeconds(3);
        
        // attack till swarm goes out of bound
        isVulnerable = false;
        stateUpdate = Attack;
        flickerCoroutine = StartCoroutine(AttackSequenceIndicator());
        while (Mathf.Abs(transform.position.y) < CombatStageManager.Instance.VerticalUpperBound && 
            Mathf.Abs(transform.position.x) < CombatStageManager.Instance.HorizontalUpperBound)
        {
            yield return new WaitForEndOfFrame();
        }    
        
        List<Vector2> path = new List<Vector2>() {  transform.position, originalPosition };

        returnLine = new BezierCurve(path.ToArray());

        stateUpdate = Return;

        while (timeSinceReturnStart <= returnTime)
        {
            yield return new WaitForEndOfFrame();
        }
        StopCoroutine(flickerCoroutine);
        
        stateUpdate = null;
        
        //resetting variables for the next attack
        ResetOpacity();
        transform.position = originalPosition;
        swarmSequenceStarted = false;
        alreadyHitPlayer = false;
        timeSinceReturnStart = 0;
        returnLine = null;
        isVulnerable = true;
        flickerCoroutine = null;
    }

    IEnumerator AttackSequenceIndicator(){
        float step = 0;
        float stepTime = 0.4f;
        while(true){
            spriteRenderer.color = step % 2 == 0 ? Color.white : new Color(0.4f, 0.4f, 0.4f, 0.6f);
            yield return new WaitForSeconds(stepTime);
            step++;
        }
    }

    IEnumerator FusionSequence(){
        StopCoroutine(attackCoroutine);
        stateUpdate = null;
        fusionSequenceStarted = true;
        isVulnerable = false;
        boxCollider.enabled = false;
        stateUpdate = TrackGiantSpawnPoint;
        flickerCoroutine = StartCoroutine(AttackSequenceIndicator());
        yield return new WaitForSeconds(2);
        
        stateUpdate = Attack;
        while (Vector2.Distance(transform.position, new Vector2(0, 2)) > 0.1f)
        {
            yield return new WaitForEndOfFrame();
        }

        transform.position = fusionLocation;
        stateUpdate = null;
        // yield return new WaitForSeconds(4);         
        
        // gameObject.SetActive(false);
        onFusionComplete.Invoke();
        Destroy(gameObject);
        // StopAllCoroutines();
    }
    #endregion

    #region "Helper funcs (to be used in attack sequence or fusion sequence)"
    void TrackPlayer()
    {
        LookAtTarget(CombatStageManager.Instance.PlayerTransform.position);
    }

    void TrackGiantSpawnPoint(){
        Vector3 giantSpawnPoint = new Vector3(0,2,0);
        LookAtTarget(giantSpawnPoint); 
    }

    void LookAtTarget(Vector3 target)
    {
        Vector2 direction = target - transform.position;
        float angleRadians = Mathf.Atan2(direction.y, direction.x);
        float angleDegrees = angleRadians * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleDegrees));
    }

    void Attack()
    {
        if (!fusionSequenceStarted && statusEffectHandler.HasStatusCondition(StatusEffect.Stun))
        {
            return;
        }

        transform.Translate(Vector2.right * attackMoveSpeed * Time.deltaTime * swarmDirection);
    }

    void Return()
    {
        timeSinceReturnStart += Time.deltaTime;
        Vector2 nextPos = returnLine.GetPosition(timeSinceReturnStart / returnTime);
        LookAtTarget(nextPos);
        transform.position = nextPos;
        alreadyHitPlayer = false;
    }

    void ResetOpacity(){
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    #endregion
}
