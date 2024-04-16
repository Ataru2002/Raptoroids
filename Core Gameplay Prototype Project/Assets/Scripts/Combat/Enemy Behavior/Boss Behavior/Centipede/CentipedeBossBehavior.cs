using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeBossBehavior : BossBehavior
{
    [SerializeField] ProjectileSpawner leftSpawner;
    [SerializeField] ProjectileSpawner rightSpawner;
    [SerializeField] StrafeEnemyBehavior strafeBehavior;

    const int legPairCount = 5;
    [SerializeField] CentipedeLegBehavior[] centipedeLeftLegs;
    [SerializeField] CentipedeLegBehavior[] centipedeRightLegs;

    bool legBarrageStarted = false;

    public bool strafing = false;
    float remainingHealthRatio = 1f;

    void Awake()
    {
        transitionConditions = new List<System.Func<bool>>
        {
            StateTransition1,
            StateTransition2
        };

        stateExecute = State1Execute;
    }

    public void UpdateHealthRatio(float val)
    {
        remainingHealthRatio = val;
        TryTransition();
    }

    public void UpdateStrafeStatus(bool val)
    {
        strafing = val;
        TryTransition();
    }

    bool StateTransition1()
    {
        if (remainingHealthRatio > 0.5f)
        {
            return false;
        }

        stateExecute = State2Execute;

        leftSpawner.enabled = true;
        rightSpawner.enabled = true;
        strafeBehavior.enabled = true;

        return true;
    }

    bool StateTransition2()
    {
        if (remainingHealthRatio > 0.4f || strafing)
        {
            return false;
        }

        stateExecute = State3Execute;

        defaultProjectileSpawn.ResetShotTimer();
        leftSpawner.ResetShotTimer();
        rightSpawner.ResetShotTimer();

        return true;
    }

    void State1Execute()
    {
        if (!FinalPositionReached)
        {
            return;
        }

        if (!legBarrageStarted)
        {
            StartCoroutine(LegBarrage());
        }
    }

    IEnumerator LegBarrage()
    {
        legBarrageStarted = true;

        for (int i = 0; i < legPairCount; i++)
        {
            CentipedeLegBehavior leftLeg = centipedeLeftLegs[i];
            CentipedeLegBehavior rightLeg = centipedeRightLegs[i];

            if (leftLeg != null)
            {
                leftLeg.TryStartAttack();
            }

            if (rightLeg != null)
            {
                rightLeg.TryStartAttack();
            }

            yield return new WaitForSeconds(1.2f);
        }

        legBarrageStarted = false;
    }

    void State2Execute()
    {
        leftSpawner.TryShoot();
        rightSpawner.TryShoot();
    }

    void State3Execute()
    {
        State2Execute();
        trackPlayer = strafing;
        
        if (!trackPlayer)
        {
            transform.rotation = Quaternion.Euler(0, 0, 270);
        }
    }
}
