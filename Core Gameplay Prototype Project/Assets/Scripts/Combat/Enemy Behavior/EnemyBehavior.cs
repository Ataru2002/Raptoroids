using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] protected bool trackPlayer;

    float timeSinceSpawn = 0;
    [SerializeField] protected float timeToFinalPosition = 0;
    
    protected Vector2 finalPosition;
    public Vector2 FinalPosition { get { return finalPosition; } }

    [SerializeField] protected Weapon defaultWeapon;

    protected BezierCurve path;

    public bool FinalPositionReached { get { return timeSinceSpawn >= timeToFinalPosition; } }

    protected StatusEffectHandler statusHandler;

    protected void Awake()
    {
        statusHandler = GetComponent<StatusEffectHandler>();
    }

    // Update is called once per frame
    protected void Update()
    {
        if (statusHandler != null && statusHandler.HasStatusCondition(StatusEffect.Stun))
        {
            return;
        }

        if (!FinalPositionReached)
        {
            timeSinceSpawn += Time.deltaTime;
            
            // Ensure the timer doesn't count beyond the specified limit,
            // since this will be used to determine the enemy's current position.
            if (timeSinceSpawn >= timeToFinalPosition)
            {
                timeSinceSpawn = timeToFinalPosition;
            }

            transform.position = path.GetPosition(timeSinceSpawn / timeToFinalPosition);
        }

        if (trackPlayer)
        {
            Vector2 posDelta = (CombatStageManager.Instance.PlayerTransform.position - transform.position).normalized;
            float angle = Mathf.Atan2(posDelta.y, posDelta.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        if (defaultWeapon != null)
        {
            defaultWeapon.TryShoot();
        }
    }

    public void SetPath(Vector2[] points)
    {
        path = new BezierCurve(points);
        finalPosition = points[points.Length - 1];
    }

    protected void ResetPathProgress()
    {
        timeSinceSpawn = 0;
    }
}

public class BezierCurve
{
    Vector2[] controlPoints;

    float length = 0;
    public float Length
    {
        get
        {
            return length == 0 ? EstimateLength() : length;
        }
    }

    public BezierCurve(Vector2[] points)
    {
        controlPoints = points;
    }

    public Vector2 GetPosition(float timeRatio)
    {
        Vector2 position = Vector2.zero;
        int n = controlPoints.Length - 1;
        for (int i = 0; i <= n; i++)
        {
            float coefficient = Combination(n, i) * Mathf.Pow(1 - timeRatio, n - i) * Mathf.Pow(timeRatio, i);
            position += controlPoints[i] * coefficient;
        }
        return position;
    }

    float EstimateLength()
    {
        const float epsilon = 1e-3f;
        float result = 0;
        
        for (float t = epsilon; t <= 1; t += epsilon)
        {
            Vector2 prev = GetPosition(t - epsilon);
            Vector2 current = GetPosition(t);
            result += Vector2.Distance(prev, current);
        }
        return length = result;
    }

    int Combination(int n, int r)
    {
        int result = Factorial(n) / (Factorial(r) * Factorial(n - r));
        return result;
    }

    int Factorial(int n)
    {
        int result = 1;
        int i = 1;
        while (i <= n)
        {
            result *= i;
            i += 1;
        }
        return result;
    }
}
