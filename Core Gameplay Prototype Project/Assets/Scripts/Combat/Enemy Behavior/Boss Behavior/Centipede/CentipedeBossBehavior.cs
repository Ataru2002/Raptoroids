using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeBossBehavior : BossBehavior
{
    [SerializeField] CentipedePincerBehavior leftPincer;
    [SerializeField] CentipedePincerBehavior rightPincer;
    [SerializeField] StrafeEnemyBehavior strafeBehavior;

    const int legPairCount = 5;
    [SerializeField] CentipedeLegBehavior[] centipedeLeftLegs;
    [SerializeField] CentipedeLegBehavior[] centipedeRightLegs;

    bool legBarrageStarted = false;
    bool pincersFiring = false;

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

        strafeBehavior.enabled = true;

        return true;
    }

    bool StateTransition2()
    {
        if (remainingHealthRatio > 0.1f || strafing)
        {
            return false;
        }

        stateExecute = State3Execute;

        // defaultProjectileSpawn.ResetShotTimer();

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
        if (!pincersFiring)
        {
            StartCoroutine(PincerFireSequence());
        }
    }

    IEnumerator PincerFireSequence()
    {
        pincersFiring = true;

        leftPincer.PlayAttackSequence();
        yield return new WaitForSeconds(0.6f);
        rightPincer.PlayAttackSequence();
        yield return new WaitForSeconds(0.6f);

        pincersFiring = false;
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
