using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBeeBehavior : EnemyBehavior, IBulletHittable
{
    QueenBeeBossBehavior boss;
    public static Vector2 meatShieldPos = new Vector2(0, 0.5f);
    public bool isMeatShield = false;
    bool bossNotified = false;

    int guardHealth = 5;

    void Awake()
    {
        boss = FindFirstObjectByType<QueenBeeBossBehavior>();
    }

    new void Update()
    {
        base.Update();
        if (isMeatShield && !bossNotified && FinalPositionReached)
        {
            bossNotified = true;
            boss.NotifyMeatShieldInPosition();
        }
    }

    public void OnBulletHit()
    {
        if (--guardHealth <= 0)
        {
            boss.NotifyGuardDown(this);
            Destroy(gameObject);
        }
    }

    public void AppointMeatShield()
    {
        isMeatShield = true;

        SetPath(new Vector2[] { transform.position, meatShieldPos });
        ResetPathProgress();
    }
}
