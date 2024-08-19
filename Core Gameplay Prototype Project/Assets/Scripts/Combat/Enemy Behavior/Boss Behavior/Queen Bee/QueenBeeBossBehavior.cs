using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class QueenBeeBossBehavior : BossBehavior
{
    const int maxGuardCount = 3;
    bool meatShieldActive = false;
    
    int wingsRemaining = 2;

    List<GuardBeeBehavior> guards = new List<GuardBeeBehavior>(maxGuardCount);
    bool canSpawnGuards = true;

    bool feeding = false;
    bool chasing = false;
    bool chaseMessageDisplayed = false;
    bool wingDeployDisplayed = false;

    Vector2 mark;
    bool markReached = false;

    Coroutine runningCoroutine;

    [SerializeField] QueenBeeWingBehavior[] wings;
    [SerializeField] GameObject guardPrefab;

    new void Awake()
    {
        base.Awake();
        nonLinearTransitionConditions = new Dictionary<int, Dictionary<int, Func<bool>>>();

        nonLinearTransitionConditions[0] = new Dictionary<int, Func<bool>>();
        nonLinearTransitionConditions[0][1] = EnterChaseState;

        nonLinearTransitionConditions[1] = new Dictionary<int, Func<bool>>();
        nonLinearTransitionConditions[1][0] = EnterIdleState;

        CombatStageManager.Instance.SetBossHintTable("QueenBeeHints");
        CombatStageManager.Instance.RegisterEnemyTransform(transform);

        stateExecute = IdleUpdate;
    }

    void Start()
    {
        CombatStageManager.Instance.DisplayBossHint("hint01");
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

        if (!chaseMessageDisplayed)
        {
            chaseMessageDisplayed = true;
            CombatStageManager.Instance.DisplayBossHint("hint03");
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

        CombatSFXManager.PlaySoundAtLocation($"Pop{1 + UnityEngine.Random.Range(0, 2)}", transform.position);

        DeployWings();
    }
    #endregion

    #region EXECUTION_CHASE
    void ChaseUpdate()
    {
        if (!chasing)
        {
            runningCoroutine = StartCoroutine(ChaseSequence());
        }

        // Update the mark only while not stunned so that the stun has a noticeable effect
        if (trackPlayer && !statusHandler.HasStatusCondition(StatusEffect.Stun))
        {
            mark = CombatStageManager.Instance.PlayerTransform.position;
        }
    }

    void RunUpdate()
    {
        transform.Translate(Vector2.right * (3f + 5f * wingsRemaining) * Time.deltaTime);
        markReached = Vector2.Distance(transform.position, mark) <= 0.3f;
    }

    void RepositionUpdate()
    {
        Vector2 currentPos = transform.position;

        Update();

        float angle = Mathf.Atan2(transform.position.y - currentPos.y, transform.position.x - currentPos.x);
        angle *= Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (FinalPositionReached)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 270f);
            TryTransitionTo(0);
        }
    }

    IEnumerator ChaseSequence()
    {
        chasing = true;
        trackPlayer = true;
        markReached = false;

        yield return new WaitForSeconds(1);
        trackPlayer = false;

        yield return new WaitUntil(() => !statusHandler.HasStatusCondition(StatusEffect.Stun));

        stateExecute = RunUpdate;
        feeding = true;

        while (!markReached)
        {
            yield return null;
        }

        feeding = false;
        stateExecute = ChaseUpdate;
        transform.position = mark;
        yield return new WaitForSeconds(2);
        chasing = false;
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
            if (!wingDeployDisplayed)
            {
                wingDeployDisplayed = true;
                CombatStageManager.Instance.DisplayBossHint("hint02");
            }

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
            if (wing != null)
            {
                Destroy(wing.gameObject);
            }
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!feeding)
        {
            return;
        }

        if (collision.tag == "Player")
        {
            CombatSFXManager.PlaySoundAtLocation("Crunch", transform.position);
            canSpawnGuards = true;

            IBulletHittable hitComponent = collision.GetComponent<IBulletHittable>();
            if (hitComponent != null)
            {
                hitComponent.OnBulletHit();
            }

            StopCoroutine(runningCoroutine);

            feeding = false;
            chasing = false;
            trackPlayer = false;

            List<Vector2> path = new List<Vector2>
            {
                transform.position,
                FinalPosition
            };

            Vector2 mid = ((Vector2)transform.position + FinalPosition) / 2f;
            Vector2 perp = Vector2.Perpendicular((Vector2)transform.position - FinalPosition) * 0.5f;
            Vector2 curvePoint = mid + perp;
            path.Insert(1, curvePoint);

            SetPath(path.ToArray());

            ResetPathProgress();
            timeToFinalPosition = 1.3f;
            stateExecute = RepositionUpdate;
        }
    }
}
