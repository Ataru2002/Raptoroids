using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : Bullet
{
    [Tooltip("Time in seconds before the homing projectile begins seeking a target")]
    [SerializeField] float seekDelay;

    [SerializeField] float angularVelocity;

    static GameObject[] enemies;
    Transform target = null;
    Coroutine targeting = null;

    new void Awake()
    {
        base.Awake();
    }

    new void Start()
    {
        base.Start();
        targeting = StartCoroutine(SetTarget());
    }

    // Update is called once per frame
    new void Update()
    {
        // Projectile guidance adapted from Brackeys tutorial at https://youtu.be/0v_H3oOR0aU?si=yT1-L-dLfh7VT4V4
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            float angleDelta = Vector3.Cross(direction, transform.up).z;
            transform.Rotate(0, 0, -angleDelta * angularVelocity * Time.deltaTime);
        }

        transform.Translate(speed * Time.deltaTime * Vector2.up);

        if (!spriteRenderer.isVisible)
        {
            despawn(gameObject);
        }
    }

    IEnumerator SetTarget()
    {
        yield return new WaitForSeconds(seekDelay);

        if (target == null)
        {
            if (!isPlayerBullet)
            {
                target = CombatStageManager.Instance.PlayerTransform;
            }
            else
            {
                target = CombatStageManager.Instance.GetClosestEnemyTransform(transform.position);
            }
        }
    }

    new public void OnGetFromPool()
    {
        base.OnGetFromPool();
        transform.rotation = Quaternion.identity;
        targeting = StartCoroutine(SetTarget());
    }

    new public void OnReleaseToPool()
    {
        base.OnReleaseToPool();
        StopCoroutine(targeting);
        target = null;
    }
}
