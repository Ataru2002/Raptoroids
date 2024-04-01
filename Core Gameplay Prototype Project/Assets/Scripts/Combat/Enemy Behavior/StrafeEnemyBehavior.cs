using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EnemyBehavior))]
public class StrafeEnemyBehavior : MonoBehaviour
{
    EnemyBehavior enemyBehavior;

    // Using the sine function, the enemy goes to the right, 
    // then to the left, then back to the center to complete the cycle.
    [SerializeField] float strafeCycleTime;

    // Invert strafe direction (i.e. start by going to the left instead)
    [SerializeField] bool invertStrafe = false;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        if (invertStrafe)
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
        if (sinceStrafeStart >= strafeCycleTime)
        {
            if (!strafeStarted)
            {
                strafeStarted = true;
                OnStrafeStart.Invoke();
            }

            sinceStrafeStart = 0;
            nextStrafeStart = Time.time + strafePauseDurationAverage + Random.Range(-strafePauseDurationMaxDelta, strafePauseDurationMaxDelta);
        }
        else
        {
            sinceStrafeStart += Time.deltaTime;
            float nextX = enemyBehavior.FinalPosition.x + strafeDirection * Mathf.Sin(2 * Mathf.PI * sinceStrafeStart / strafeCycleTime);
            transform.position = new Vector3(nextX, transform.position.y, transform.position.z);
            strafeStarted = false;
            OnStrafeEnd.Invoke();
        }
    }
}
