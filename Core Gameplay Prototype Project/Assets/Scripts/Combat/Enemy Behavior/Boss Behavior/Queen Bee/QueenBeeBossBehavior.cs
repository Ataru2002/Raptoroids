using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QueenBeeBossBehavior : BossBehavior
{
    const int maxGuardCount = 3;
    int guardsRemaining = 0;
    
    int wingsRemaining = 2;
    int wingsAttached = 2;

    List<GuardBeeBehavior> guards = new List<GuardBeeBehavior>(maxGuardCount);
    bool playerCaught = false;

    [SerializeField] GameObject body;
    [SerializeField] QueenBeeWingBehavior[] wings;

    #region TRANSITIONS

    #region AUXILIARY_FUNCTIONS
    bool WingsReturned()
    {
        return wingsRemaining == 0 || wingsAttached == wingsRemaining;
    }
    #endregion

    #region SRC_STATE_0
    bool EnterChaseState()
    {
        if (!WingsReturned())
        {
            return false;
        }

        return guardsRemaining == 0;
    }
    #endregion

    #region SRC_STATE_1
    bool EnterIdleState()
    {
        return playerCaught;
    }
    #endregion

    #endregion

    #region EXECUTION
    #endregion
}
