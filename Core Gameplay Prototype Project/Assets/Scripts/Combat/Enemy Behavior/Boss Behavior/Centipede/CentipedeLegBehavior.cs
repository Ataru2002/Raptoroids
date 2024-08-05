using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeLegBehavior : MonoBehaviour
{
    Transform parentTransform;

    [SerializeField] Material defaultMat;
    [SerializeField] Material attackMat;
    SpriteRenderer spriteRenderer;

    [SerializeField] bool isLeftLeg = false;
    float legDirection = -1f;

    EnemyHealth centipedeHP;
    int legHP = 6;

    const int pointsAwarded = 50;

    const float moveSpeed = 1f;
    const float attackMoveSpeed = 4f;
    [SerializeField] float bodySeparationDistance = 1f;
    Vector2 originalPosition;

    StatusEffectHandler statusHandler;

    bool attackStarted = false;
    bool alreadyHitPlayer = false;

    float distanceTraveled = 0f;

    BezierCurve returnCurve = null;
    const float returnTime = 3f;
    float timeSinceReturnStart = 0;

    Action stateUpdate = null;

    bool destroyed = false;

    void Awake()
    {
        parentTransform = transform.parent;
        legDirection = isLeftLeg ? 1f : -1f;
        originalPosition = transform.position;
        centipedeHP = GetComponentInParent<EnemyHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        statusHandler = GetComponentInChildren<StatusEffectHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stateUpdate != null && !statusHandler.HasStatusCondition(StatusEffect.Stun))
        {
            stateUpdate();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!alreadyHitPlayer && other.tag == "Player")
        {
            other.GetComponent<IBulletHittable>().OnBulletHit();
            alreadyHitPlayer = true;
        }
    }

    public void NotifyLegHit(int damage)
    {
        if (destroyed)
        {
            return;
        }

        int multiplier = attackStarted ? 2 : 1;

        // Limit damage to remaining HP to avoid changing states
        // before all relevant parts are destroyed
        int finalDamage = Mathf.Clamp(damage * multiplier, 1, legHP);
        legHP -= finalDamage;
        CombatStageManager.Instance.UpdateScore(pointsAwarded);
        centipedeHP.TakeDamage(finalDamage);

        if (legHP <= 0)
        {
            destroyed = true;
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    public void TryStartAttack()
    {
        if (attackStarted)
        {
            return;
        }
        
        StartCoroutine(AttackSequence());
    }

    IEnumerator AttackSequence()
    {
        attackStarted = true;

        transform.parent = null;
        spriteRenderer.material = attackMat;
        stateUpdate = MoveFromBody;
        while (distanceTraveled < bodySeparationDistance)
        {
            yield return new WaitForEndOfFrame();
        }

        stateUpdate = TrackPlayer;
        yield return new WaitForSeconds(2);

        stateUpdate = null;
        yield return new WaitUntil(() => !statusHandler.HasStatusCondition(StatusEffect.Stun));

        stateUpdate = Attack;
        while (Mathf.Abs(transform.position.y) < CombatStageManager.Instance.VerticalUpperBound && 
            Mathf.Abs(transform.position.x) < CombatStageManager.Instance.HorizontalUpperBound)
        {
            yield return new WaitForEndOfFrame();
        }

        transform.parent = parentTransform;
        
        List<Vector2> path = new List<Vector2>() {  transform.position, originalPosition };

        Vector2 mid = ((Vector2)transform.position + originalPosition) / 2f;
        Vector2 perp = Vector2.Perpendicular((Vector2)transform.position - originalPosition) * legDirection * 0.5f;
        Vector2 curvePoint = mid + perp;
        path.Insert(1, curvePoint);

        returnCurve = new BezierCurve(path.ToArray());
        stateUpdate = ReturnToBody;
        
        while (timeSinceReturnStart <= returnTime)
        {
            yield return new WaitForEndOfFrame();
        }
        stateUpdate = null;
        spriteRenderer.material = defaultMat;
        transform.localEulerAngles = Vector3.zero;
        transform.position = originalPosition;

        // Cooldown
        yield return new WaitForSeconds(3);

        attackStarted = false;
        alreadyHitPlayer = false;
        distanceTraveled = 0;
        timeSinceReturnStart = 0;
        returnCurve = null;
    }

    void MoveFromBody()
    {
        float distance = moveSpeed * Time.deltaTime;
        distanceTraveled += distance;

        Vector2 dir = Vector2.up * distance * legDirection;
        transform.Translate(dir);
    }

    void TrackPlayer()
    {
        LookAtTarget(CombatStageManager.Instance.PlayerTransform.position);
    }

    void Attack()
    {
        transform.Translate(Vector2.up * attackMoveSpeed * Time.deltaTime * legDirection);
    }

    void ReturnToBody()
    {
        timeSinceReturnStart += Time.deltaTime;
        Vector2 nextPos = returnCurve.GetPosition(timeSinceReturnStart / returnTime);
        LookAtTarget(nextPos);
        transform.position = nextPos;
    }

    void LookAtTarget(Vector3 target)
    {
        Vector2 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x);
        angle += -legDirection * (Mathf.PI / 2f);
        angle *= Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
