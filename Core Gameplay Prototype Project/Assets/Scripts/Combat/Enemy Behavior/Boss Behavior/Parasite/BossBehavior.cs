using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Used for analytics collection
public enum Bosses
{
    Centipede,
    Parasite,
    Mantis,
    Bee
}

public class BossBehavior : EnemyBehavior
{
    protected int bossState = 0;

    // Used for bosses whose state transition is linear
    protected List<Func<bool>> transitionConditions;

    // Used for bosses whose state transition is non-linear (e.g., cycles or branches)
    // The key for the outer dictionary describes the source state, and the key for the inner dictionary
    // describes destination state.
    protected Dictionary<int, Dictionary<int, Func<bool>>> nonLinearTransitionConditions;

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

    protected void TryTransitionTo(int destinationState)
    {
        if (!nonLinearTransitionConditions[bossState].ContainsKey(destinationState) ||
            !nonLinearTransitionConditions[bossState][destinationState]())
        {
            return;
        }

        bossState = destinationState;
    }
}
