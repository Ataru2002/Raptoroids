using System.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

// using System.Numerics;
using UnityEngine;
//99% Poom's code - Harry
public class TentacleBehavior : MonoBehaviour
{
    Transform parentTransform;
    [SerializeField] bool isLeftTentacle = false;

    float tentacleDirection = 1f;

    EnemyHealth parasiteHP;

    int tentacleHP = 3;

    const float moveSpeed = 1f;

    const float attackMoveSpeed = 8f;

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
        tentacleDirection = isLeftTentacle ? 1f : -1f;
        originalPosition = transform.position;
        parasiteHP = GetComponentInParent<EnemyHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update(){
        if(stateUpdate != null){
            stateUpdate();
        }
    }
    public void TryTentacleSequence(){
        if(attackStarted){
            return;
        }
        StartCoroutine(AttackSequence());
    }

    public void TryAttackSequenceIndicator(){
        if(indicatorStarted){
            return;
        }

        StartCoroutine(AttackSequenceIndicator());
    }

    void OnTriggerEnter2D(Collider2D other){
        if(!alreadyHitPlayer && other.tag == "Player"){
            other.GetComponent<IBulletHittable>().OnBulletHit();
        }
    }

    public void NotifytentacleHit(){
        if(vulnerable){
            int damage = 1;
            tentacleHP -= damage;
            parasiteHP.TakeDamage(damage);

            if(tentacleHP <= 0){
                
                StopAllCoroutines();
                Destroy(gameObject);
            }
        }
    }

    IEnumerator AttackSequenceIndicator(){
        float endOpacity = 1;
        float startOpacity = 0;
        float elapsedTime = 0f;
        float opacDuration = 2;
        float middleOpacity = elapsedTime/opacDuration;

        while(elapsedTime < opacDuration){
            float alpha = Mathf.Lerp(startOpacity, endOpacity, middleOpacity);
            spriteRenderer.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
        
        spriteRenderer.color = new Color(1, 1, 1, endOpacity);
    }
    IEnumerator AttackSequence(){
        attackStarted = true;
        
        transform.parent = null;
        // stateUpdate = MoveFromBody;
        // while (distanceTraveled < bodySeparationDistance)
        // {
        //     yield return new WaitForEndOfFrame();
        // }
        
        stateUpdate = TrackPlayer;

        yield return new WaitForSeconds(1);

        
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
        transform.Translate(Vector2.up * attackMoveSpeed * Time.deltaTime * tentacleDirection);
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
    }

    void LookAtTarget(Vector3 target)
    {
        Vector2 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x);
        angle += -tentacleDirection * (Mathf.PI / 2f);
        angle *= Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void TrackPlayer(){
        LookAtTarget(CombatStageManager.Instance.PlayerTransform.position);
    }



}
