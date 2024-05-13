using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QueenBeeBossBehavior : BossBehavior
{
    const int maxGuardCount = 5;
    public event Action beeGuardMoveOrderEvent;
}
