using System.Collections;
using System;
using System.Collections.Generic;

// using System.Numerics;
using UnityEngine;
using UnityEditor;
//99% Poom's code - Harry
public class TentacleBehavior : MonoBehaviour
{
    Transform parentTransform;
    [SerializeField] bool isTopTentacle = false;
    int pointsAwarded = 50;
    float tentacleDirection = 1f;


    EnemyHealth parasiteHP;
    bool tentacleDestroyed = false;
    int tentacleHP = 3;

    const float moveSpeed = 1f;

    const float attackMoveSpeed = 8f;

    StatusEffectHandler statusHandler;

    // [SerializeField] float bodySeparationDistance = 0.5f;
    Vector2 originalPosition;
    bool attackStarted = false;
    float distanceTraveled = 0f;
    bool alreadyHitPlayer = false;
    bool indicatorStarted = false;
    BezierCurve returnLine = null;
    const float returnTime = 2f;
    float timeSinceReturnStart = 0;
    SpriteRenderer spriteRenderer;
    Action stateUpdate = null;
    bool vulnerable = false;

    void Awake(){
        parentTransform = transform.parent;
        tentacleDirection = isTopTentacle ? 1f : -1f;
        originalPosition = transform.position;
        parasiteHP = GetComponentInParent<EnemyHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        statusHandler = GetComponent<StatusEffectHandler>();
    }

    void Update(){
        if(stateUpdate != null && !statusHandler.HasStatusCondition(StatusEffect.Stun)){
            stateUpdate();
        }
    }

    public void TryTentacleSequence(){
        if (attackStarted) {
            return;
        }

        StartCoroutine(AttackSequence());
    }

    public void TryAttackSequenceIndicator(){
        if(indicatorStarted) {
            return;
        }

        StartCoroutine(AttackSequenceIndicator());
    }

    void OnTriggerEnter2D(Collider2D other){
        if(!alreadyHitPlayer && other.tag == "Player"){
            other.GetComponent<IBulletHittable>().OnBulletHit();
            alreadyHitPlayer = true;
        }
    }

    public void NotifytentacleHit(int damage){
        if (tentacleDestroyed) {
            return;
        }

        if (vulnerable) {
            int finalDamage = Mathf.Clamp(damage, 1, tentacleHP);
            tentacleHP -= finalDamage;
            CombatStageManager.Instance.UpdateScore(pointsAwarded);
            parasiteHP.TakeDamage(finalDamage);

            if(tentacleHP <= 0){
                tentacleDestroyed = true;
                StopAllCoroutines();
                Destroy(gameObject);
            }
        }
    }

    IEnumerator AttackSequenceIndicator(){
        float step = 0;
        float stepTime = 0.2f;
        while(true){
            spriteRenderer.color = step % 2 == 0 ? Color.white : new Color(0.4f, 0.4f, 0.4f, 0.6f);
            yield return new WaitForSeconds(stepTime);
            step++;
        }
    }

    IEnumerator AttackSequence(){
        attackStarted = true;
        
        StartCoroutine(AttackSequenceIndicator());

        stateUpdate = TrackPlayer;

        yield return new WaitForSeconds(1);

        stateUpdate = null;
        yield return new WaitUntil(() => !statusHandler.HasStatusCondition(StatusEffect.Stun));
        
        vulnerable = true;
        stateUpdate = Attack;
        while (Mathf.Abs(transform.position.y) < CombatStageManager.Instance.VerticalUpperBound && 
            Mathf.Abs(transform.position.x) < CombatStageManager.Instance.HorizontalUpperBound)
        {
            yield return new WaitForEndOfFrame();
        }

        transform.parent = parentTransform;
        List<Vector2> path = new List<Vector2>() {  transform.position, originalPosition };

        returnLine = new BezierCurve(path.ToArray());
        stateUpdate = ReturnToBody;
        
        while (timeSinceReturnStart <= returnTime)
        {
            yield return new WaitForEndOfFrame();
        }
        stateUpdate = null;
        transform.position = originalPosition;

        ////Cooldown
        yield return new WaitForSeconds(0.5f);

        attackStarted = false;
        alreadyHitPlayer = false;
        distanceTraveled = 0;
        timeSinceReturnStart = 0;
        returnLine = null;
        vulnerable = false;
    }

    void Attack(){
        if(isTopTentacle){
            transform.Translate(Vector2.right * attackMoveSpeed * Time.deltaTime * -tentacleDirection);
        }
        else{
            transform.Translate(Vector2.right * attackMoveSpeed * Time.deltaTime * tentacleDirection);
        }
    }

    void MoveFromBody()
    {
        float distance = moveSpeed * Time.deltaTime;
        distanceTraveled += distance;

        Vector2 dir = Vector2.up * distance * tentacleDirection;
        transform.Translate(dir);
    }

    void ReturnToBody()
    {
        timeSinceReturnStart += Time.deltaTime;
        Vector2 nextPos = returnLine.GetPosition(timeSinceReturnStart / returnTime);
        LookAtTarget(nextPos);
        transform.position = nextPos;
        alreadyHitPlayer = false;
    }

    void LookAtTarget(Vector3 target)
    {
        Vector2 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x);
        angle += -tentacleDirection * (Mathf.PI / 2f) * 2;
        angle *= Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void TrackPlayer(){
        LookAtTarget(CombatStageManager.Instance.PlayerTransform.position);
    }
}
