using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceProjectile : Bullet
{
    protected int sequenceStep;

    public void SetSequenceStep(int val)
    {
        sequenceStep = val;
    }
}
