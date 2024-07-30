using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EnemyBehavior))]
public class StrafeEnemyBehavior : MonoBehaviour
{
    EnemyBehavior enemyBehavior;
    StatusEffectHandler statusHandler;

    // Using the sine function, the enemy goes to the right, 
    // then to the left, then back to the center to complete the cycle.
    [SerializeField] float strafeCycleTime;

    // Invert strafe direction (i.e. start by going to the left instead)
    [SerializeField] bool invertStrafe = false;

    public bool publicInvertStrafe = false;

    // How far the enemy will go from the center
    [SerializeField] float strafeDistance;

    [SerializeField] float strafePauseDurationAverage;
    [SerializeField] float strafePauseDurationMaxDelta;

    int strafeDirection = 1;
    float sinceStrafeStart = 0;
    float nextStrafeStart = 0;

    bool strafeStarted = false;

    public UnityEvent OnStrafeStart;
    public UnityEvent OnStrafeEnd;

    private void Awake()
    {
        enemyBehavior = GetComponent<EnemyBehavior>();
        statusHandler = GetComponent<StatusEffectHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (invertStrafe || publicInvertStrafe)
        {
            strafeDirection = -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyBehavior.FinalPositionReached && Time.time >= nextStrafeStart)
        {
            Strafe();
        }
    }

    void Strafe()
    {
        if (statusHandler != null && statusHandler.HasStatusCondition(StatusEffect.Stun))
        {
            return;
        }

        if (sinceStrafeStart >= strafeCycleTime)
        {
            sinceStrafeStart = 0;
            nextStrafeStart = Time.time + strafePauseDurationAverage + Random.Range(-strafePauseDurationMaxDelta, strafePauseDurationMaxDelta);

            OnStrafeEnd.Invoke();
            strafeStarted = false;
        }
        else
        {
            if (!strafeStarted)
            {
                strafeStarted = true;
                OnStrafeStart.Invoke();
            }

            sinceStrafeStart += Time.deltaTime;
            float nextX = enemyBehavior.FinalPosition.x + strafeDirection * strafeDistance * Mathf.Sin(2 * Mathf.PI * sinceStrafeStart / strafeCycleTime);
            transform.position = new Vector3(nextX, transform.position.y, transform.position.z);
        }
    }
}
