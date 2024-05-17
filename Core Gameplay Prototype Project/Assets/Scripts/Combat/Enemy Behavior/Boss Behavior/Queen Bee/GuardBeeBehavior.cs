using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBeeBehavior : EnemyBehavior, IBulletHittable
{
    static Vector2[] guardPositions =
    {
        new Vector2 (0, 0),
        new Vector2 (1, 0),
        new Vector2 (2, 0)
    };

    int id;

    public void DesignateGuardNumber(int guardNumber)
    {
        id = guardNumber;
    }

    new void Update()
    {

    }

    public void OnBulletHit()
    {

    }

    public void Reposition()
    {
        Vector2[] newPath = { transform.position, guardPositions[id] };
        SetPath(newPath);
        ResetPathProgress();
    }
}
