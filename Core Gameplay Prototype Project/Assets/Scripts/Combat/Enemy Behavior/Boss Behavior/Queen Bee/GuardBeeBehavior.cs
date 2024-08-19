using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBeeBehavior : EnemyBehavior, IBulletHittable
{
    QueenBeeBossBehavior boss;
    StatusEffectHandler statusEffectHandler;
    public static Vector2 meatShieldPos = new Vector2(0, 0.5f);
    public bool isMeatShield = false;
    bool bossNotified = false;

    int guardHealth = 5;

    new void Awake()
    {
        base.Awake();
        boss = FindFirstObjectByType<QueenBeeBossBehavior>();
        CombatStageManager.Instance.RegisterEnemyTransform(transform);
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

    public void OnBulletHit(int damage = 1)
    {
        guardHealth -= damage;
        if (guardHealth <= 0)
        {
            boss.NotifyGuardDown(this);
            // Remove the guard's transform from the list. Depending on how the player plays,
            // the number of guards could grow to large numbers.
            CombatStageManager.Instance.UnregisterEnemyTransform(transform);
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
