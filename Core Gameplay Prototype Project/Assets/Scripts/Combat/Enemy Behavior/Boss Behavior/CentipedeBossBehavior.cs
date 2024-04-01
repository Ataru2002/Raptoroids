using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeBossBehavior : BossBehavior
{
    [SerializeField] ProjectileSpawner leftSpawner;
    [SerializeField] ProjectileSpawner rightSpawner;
    [SerializeField] StrafeEnemyBehavior strafeBehavior;

    bool strafing = false;
    float remainingHealthRatio = 1f;

    void Awake()
    {
        transitionConditions = new List<System.Func<bool>>
        {
            StateTransition1,
        };
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
        if (remainingHealthRatio > 0.7f)
        {
            return false;
        }

        stateExecute = State2Execute;

        leftSpawner.enabled = true;
        rightSpawner.enabled = true;
        strafeBehavior.enabled = true;

        defaultProjectileSpawn.ResetShotTimer();

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
