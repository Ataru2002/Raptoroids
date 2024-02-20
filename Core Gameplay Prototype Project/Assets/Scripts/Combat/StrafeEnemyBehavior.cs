using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafeEnemyBehavior : EnemyBehavior
{
    // Using the sine function, the enemy goes to the right, 
    // then to the left, then back to the center to complete the cycle.
    [SerializeField] float strafeCycleTime;

    // How far the enemy will go from the center
    [SerializeField] float strafeDistance;

    // TODO: move this into the base EnemyBehavior class
    // so that the enemies can "fly in".
    [SerializeField] Vector2 finalPosition;

    [SerializeField] float strafePauseDurationAverage;
    [SerializeField] float strafePauseDurationMaxDelta;

    float sinceStrafeStart = 0;
    float nextStrafeStart = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextStrafeStart)
        {
            Strafe();
        }
    }

    void Strafe()
    {
        if (sinceStrafeStart >= strafeCycleTime)
        {
            sinceStrafeStart = 0;
            nextStrafeStart = Time.time + strafePauseDurationAverage + Random.Range(-strafePauseDurationMaxDelta, strafePauseDurationMaxDelta);
        }
        else
        {
            sinceStrafeStart += Time.deltaTime;
            float nextX = finalPosition.x + Mathf.Sin(2 * Mathf.PI * sinceStrafeStart / strafeCycleTime);
            transform.position = new Vector3(nextX, transform.position.y, transform.position.z);
        }
    }
}
