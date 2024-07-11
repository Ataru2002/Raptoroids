using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GiantArmBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] bool isLeftArm = false;
    [SerializeField] FadingEffect bossInvis;
    [SerializeField] EnemyHealth combinedBossHP;
    bool attackStarted = false;
    bool alreadyHitPlayer = false;
    bool armDestroyed = false;
    float armDirection = -1;
    float attackMoveSpeed = 6f;
    float distanceTraveled = 0;
    float bodySeparationDistance = 0.5f;
    float moveSpeed = 2;
    Action stateUpdate = null;
    Vector2 originalPosition;
    Vector2 attackDirection;
    float timeSinceReturnStart = 0;
    float returnTime = 1.5f;
    int pointsAwarded = 15;
    int armHP = 10;
    BezierCurve returnCurve = null;

    void Awake(){
        originalPosition = transform.position;
        armDirection = isLeftArm ?  1f : -1f;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(stateUpdate != null){
            stateUpdate();
        }
    }

    public void tryBoomerangAttack(){
        if(attackStarted){
            return;
        }

        if(gameObject.activeInHierarchy){
            StartCoroutine(boomerangAttack());
        }
        
    }
    void OnTriggerEnter2D(Collider2D other){
        if(!alreadyHitPlayer && other.tag == "Player"){
            other.GetComponent<IBulletHittable>().OnBulletHit();
            alreadyHitPlayer = true;
        }
    }
    public void NotifyArmHit(int damage){
        if(armDestroyed){
            return;
        }

        int finalDamage = Mathf.Clamp(damage, 1, armHP);
        armHP -= finalDamage;
        CombatStageManager.Instance.UpdateScore(pointsAwarded);
        combinedBossHP.TakeDamage(finalDamage);
        
        if(armHP <= 0){
            armDestroyed = true;
            // StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    IEnumerator boomerangAttack(){
        attackStarted = true;

        CombatSFXManager.PlaySoundAtLocation("MantisArmSwoosh", transform.position);

        stateUpdate = MoveFromBody;
        while(distanceTraveled < bodySeparationDistance){
            yield return new WaitForEndOfFrame();
        }
        
        stateUpdate = TrackPlayer;
        yield return new WaitForSeconds(0.2f);
        bossInvis.tryFadeOut();
        stateUpdate = Attack;

        while (Mathf.Abs(transform.position.y) < CombatStageManager.Instance.VerticalUpperBound && 
            Mathf.Abs(transform.position.x) < CombatStageManager.Instance.HorizontalUpperBound)
        {
            yield return new WaitForEndOfFrame();
        }    

        List<Vector2> path = new List<Vector2>() {  transform.position, originalPosition };
        Vector2 mid = ((Vector2)transform.position + originalPosition) / 2f;
        Vector2 perp = Vector2.Perpendicular((Vector2)transform.position - originalPosition) * armDirection * 0.5f;
        Vector2 curvePoint = mid + perp;
        path.Insert(1, curvePoint);
        
        returnCurve = new BezierCurve(path.ToArray());
        stateUpdate = ReturnToBody;
  
        while (timeSinceReturnStart <= returnTime)
        {
            yield return new WaitForEndOfFrame();
        }
        transform.position = originalPosition;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        stateUpdate = null;
        bossInvis.tryFadeIn(false);
        yield return new WaitForSeconds(1.5f);
        attackStarted = false;
        alreadyHitPlayer = false;
        distanceTraveled = 0;
        timeSinceReturnStart = 0;
        returnCurve = null;
    }

    #region HELPER FUNC
    void MoveFromBody(){
        float distance = moveSpeed * Time.deltaTime; 
        Vector2 dir = Vector2.down * distance * armDirection;
        distanceTraveled += distance;
        transform.Translate(dir);
    }

    void ReturnToBody()
    {
        alreadyHitPlayer = false;
        timeSinceReturnStart += Time.deltaTime;
        Vector2 nextPos = returnCurve.GetPosition(timeSinceReturnStart / returnTime);
        transform.position = nextPos;
        spinToWin();
    }
    
    void Attack()
    {
        print(attackDirection);
        transform.Translate(attackDirection * attackMoveSpeed * Time.deltaTime, Space.World);
        spinToWin();
    }
    void spinToWin(){
        transform.Rotate(Vector3.forward, 360 * Time.deltaTime * 2 * armDirection);
    }
    void TrackPlayer()
    {
        LookAtTarget(CombatStageManager.Instance.PlayerTransform.position);
    }
    void LookAtTarget(Vector3 target)
    {
        attackDirection = target - transform.position;
        float angle = Mathf.Atan2(attackDirection.y, attackDirection.x);
        angle += armDirection * (Mathf.PI / 2f);
        angle *= Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    #endregion
}
