using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : EnemyBehavior
{
    protected int bossState = 0;
    protected List<Func<bool>> transitionConditions;

    protected Action stateExecute;

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (stateExecute != null)
        {
            stateExecute();
        }
    }

    protected void TryTransition()
    {
        if (bossState >= transitionConditions.Count || !transitionConditions[bossState]())
        {
            return;
        }

        bossState++;
    }
}
