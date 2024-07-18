using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvePairProjectile : SequenceProjectile
{
    [Tooltip("Positions of control points relative to the initial position (excluding endpoint)")]
    [SerializeField] Vector2[] pathOffsets = new Vector2[2];
    BezierCurve curvePath;
    Vector2 postCurveDirection;

    [Tooltip("How far 'up' will the projectile be when it comes back to the initial x-coordinate in object space")]
    [SerializeField] float range;

    float curveTime;
    float timeElapsed = 0f;

    Spin spinControl;
    TrailRenderer trailRenderer;

    new void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spinControl = GetComponentInChildren<Spin>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
    }

    // Update is called once per frame
    new void Update()
    {
        if (timeElapsed <= curveTime)
        {
            transform.position = curvePath.GetPosition(timeElapsed / curveTime);
        }
        else
        {
            if (!spriteRenderer.isVisible)
            {
                despawn(gameObject);
            }
            else
            {
                transform.Translate(speed * Time.deltaTime * postCurveDirection.normalized, Space.World);
            }
        }

        timeElapsed += Time.deltaTime;
    }

    public void SetPath()
    {
        float curveDirection = sequenceStep == 0 ? 1 : -1;

        Vector2 point1Offset = pathOffsets[0].x * curveDirection * transform.right + pathOffsets[0].y * transform.up;
        Vector2 point1 = (Vector2)transform.position + point1Offset;

        Vector2 point2Offset = pathOffsets[1].x * curveDirection * transform.right + pathOffsets[1].y * transform.up;
        Vector2 point2 = (Vector2)transform.position + point2Offset;

        Vector2 end = transform.position + range * transform.up;
        Vector2[] controlPoints = new Vector2[4]
        {
            transform.position,
            point1,
            point2,
            end
        };

        curvePath = new BezierCurve(controlPoints);

        postCurveDirection = (end - point2).normalized;
        curveTime = curvePath.Length / speed;
    }

    IEnumerator ResetPath()
    {
        trailRenderer.Clear();
        trailRenderer.emitting = false;

        // Wait for end of frame so that the projectile is moved into correct starting position
        // after being retrieved from pool.
        yield return new WaitForEndOfFrame();

        SetPath();
        timeElapsed = 0f;
        trailRenderer.emitting = true;

        spinControl.SetClockwise(sequenceStep == 0);
        spriteRenderer.flipX = sequenceStep != 0;
    }

    new public void OnGetFromPool()
    {
        base.OnGetFromPool();
        StartCoroutine(ResetPath());
    }

    new public void OnReleaseToPool()
    {
        base.OnReleaseToPool();
        StopAllCoroutines();
    }
}
