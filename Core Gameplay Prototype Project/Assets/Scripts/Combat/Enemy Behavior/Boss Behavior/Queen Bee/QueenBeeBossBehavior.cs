using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QueenBeeBossBehavior : BossBehavior
{
    const int maxGuardCount = 3;
    bool meatShieldActive = false;
    
    int wingsRemaining = 2;

    List<GuardBeeBehavior> guards = new List<GuardBeeBehavior>(maxGuardCount);
    bool canSpawnGuards = true;

    [SerializeField] QueenBeeWingBehavior[] wings;
    [SerializeField] GameObject guardPrefab;

    void Awake()
    {
        nonLinearTransitionConditions = new Dictionary<int, Dictionary<int, Func<bool>>>();

        nonLinearTransitionConditions[0] = new Dictionary<int, Func<bool>>();
        nonLinearTransitionConditions[0][1] = EnterChaseState;

        nonLinearTransitionConditions[1] = new Dictionary<int, Func<bool>>();
        nonLinearTransitionConditions[1][0] = EnterIdleState;

        stateExecute = IdleUpdate;
    }

    #region TRANSITIONS

    #region AUXILIARY_FUNCTIONS
    bool WingsReturned()
    {
        //print("Wings remaining: " + wingsRemaining);
        return wingsRemaining == 0 || WingsAttached() == wingsRemaining;
    }

    int WingsAttached()
    {
        int count = 0;
        foreach (QueenBeeWingBehavior wing in wings)
        {
            if (wing.attached)
            {
                count += 1;
            }
        }
        return count;
    }
    #endregion

    #region SRC_STATE_0
    bool EnterChaseState()
    {
        if (!WingsReturned())
        {
            return false;
        }

        if (guards.Count != 0)
        {
            return false;
        }

        stateExecute = ChaseUpdate;
        trackPlayer = true;
        return true;
    }
    #endregion

    #region SRC_STATE_1
    bool EnterIdleState()
    {
        if (!canSpawnGuards)
        {
            return false;
        }

        stateExecute = IdleUpdate;
        return true;
    }
    #endregion

    #endregion

    #region EXECUTION_IDLE
    void IdleUpdate()
    {
        if (canSpawnGuards)
        {
            SpawnGuards();
        }
    }

    void DeployWings()
    {
        foreach (QueenBeeWingBehavior wing in wings)
        {
            if (wing != null)
            {
                wing.Deploy();
            }
        }
    }

    void RecallWings()
    {
        foreach (QueenBeeWingBehavior wing in wings)
        {
            if (wing != null)
            {
                wing.Recall();
            }
        }
    }

    void SpawnGuards()
    {
        canSpawnGuards = false;

        GameObject meatShield = Instantiate(guardPrefab, transform.position, Quaternion.Euler(0, 0, 270));
        GuardBeeBehavior meatShieldBehavior = meatShield.GetComponent<GuardBeeBehavior>();
        meatShieldBehavior.AppointMeatShield();

        guards.Add(meatShieldBehavior);

        for (int i = 0; i < maxGuardCount - 1; i++)
        {
            GameObject guard = Instantiate(guardPrefab);
            GuardBeeBehavior guardBehavior = guard.GetComponent<GuardBeeBehavior>();

            guards.Add(guardBehavior);

            float lowX = i == 0 ? 0.2f : 0.7f;
            float hiX = i == 0 ? 0.3f : 0.8f;
            float randX = UnityEngine.Random.Range(lowX, hiX);
            float randY = UnityEngine.Random.Range(0.1f, 0.4f);
            Vector3 selectedPos = Camera.main.ViewportToWorldPoint(new Vector3(randX, randY, 10));
            print(selectedPos);
            guardBehavior.SetPath(new Vector2[] { transform.position, selectedPos });
        }

        DeployWings();
    }
    #endregion

    #region EXECUTION_CHASE
    void ChaseUpdate()
    {
        
    }
    #endregion

    #region PARTS_COMMS
    public void NotifyGuardDown(GuardBeeBehavior guard)
    {
        print("Received notification of guard down");
        guards.Remove(guard);
        print("Guards remaining: " + guards.Count);

        meatShieldActive = !guard.isMeatShield;

        // Make sure to recall deployed wings to prepare to chase player
        if (guards.Count == 0)
        {
            RecallWings();
        }
        else if (!meatShieldActive)
        {
            DeployWings();
            AppointMeatShield();
        }

        TryTransitionTo(1);
    }

    void AppointMeatShield()
    {
        float minDistToQueen = float.MaxValue;
        GuardBeeBehavior newMeatShield = null;

        foreach (GuardBeeBehavior guard in guards)
        {
            float distance = Vector2.Distance(guard.transform.position, transform.position);
            if (distance < minDistToQueen)
            {
                minDistToQueen = distance;
                newMeatShield = guard;
            }
        }

        newMeatShield.AppointMeatShield();
    }

    public void NotifyMeatShieldInPosition()
    {
        RecallWings();
    }

    public void NotifyWingReattach()
    {
        print("Got notification of wing reattachment");
        TryTransitionTo(1);
    }

    public void NotifyWingDestroyed()
    {
        wingsRemaining -= 1;
        TryTransitionTo(1);
    }

    public void OnDefeat()
    {
        foreach (GuardBeeBehavior guard in guards)
        {
            Destroy(guard.gameObject);
        }

        foreach (QueenBeeWingBehavior wing in wings)
        {
            Destroy(wing.gameObject);
        }
    }
    #endregion
}
