using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafeEnemyBehavior : EnemyBehavior
{
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

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        if (invertStrafe)
        {
            strafeDirection = -1;
        }
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (FinalPositionReached && Time.time >= nextStrafeStart)
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
            float nextX = finalPosition.x + strafeDirection * Mathf.Sin(2 * Mathf.PI * sinceStrafeStart / strafeCycleTime);
            transform.position = new Vector3(nextX, transform.position.y, transform.position.z);
        }
    }
}
