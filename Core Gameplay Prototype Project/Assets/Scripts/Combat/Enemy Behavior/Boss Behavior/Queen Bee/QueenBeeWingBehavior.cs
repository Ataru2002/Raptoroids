using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenBeeWingBehavior : MonoBehaviour, IBulletHittable
{
    QueenBeeBossBehavior beeBoss;
    public bool attached { get; private set; } = true;
    int wingHealth = 8;
    bool wingDestroyed = false;

    [SerializeField] Vector2[] endPoints;
    [SerializeField] float finalAngle;
    int changeDirection = 0;
    const float travelTime = 1.2f;
    float timeElapsed = 0;

    void Awake()
    {
        beeBoss = GetComponentInParent<QueenBeeBossBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime * changeDirection;
        timeElapsed = Math.Clamp(timeElapsed, 0, travelTime);
        if (!attached && timeElapsed == 0)
        {
            attached = true;
            beeBoss.NotifyWingReattach();
        }

        Vector2 newPos = Vector2.Lerp(endPoints[0], endPoints[1], timeElapsed / travelTime);
        transform.localPosition = new Vector3(newPos.x, newPos.y, -0.1f);

        float angle = Mathf.Lerp(0, finalAngle, timeElapsed / travelTime);
        transform.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void OnBulletHit(int damage = 1)
    {
        wingHealth -= damage;
        if (wingHealth <= 0 && !wingDestroyed)
        {
            wingDestroyed = true;

            beeBoss.NotifyWingDestroyed();

            Destroy(gameObject);
        }
    }

    public void Deploy()
    {
        attached = false;
        changeDirection = 1;
    }

    public void Recall()
    {
        changeDirection = -1;
    }
}
