using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeamSource : Weapon
{
    [SerializeField] float laserRange;
    [SerializeField] float laserBreadth;
    [SerializeField] float laserTickTime;

    bool laserActive = false;
    SpriteRenderer beamRenderer;
    float timeSinceLastTick;

    ContactFilter2D contactFilter = new ContactFilter2D();

    void Awake()
    {
        beamRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        
        if (laserTickTime == 0)
        {
            laserTickTime = 1f / effectiveFireRate;
        }

        timeSinceLastTick = laserTickTime;
        
        LayerMask mask = isPlayer ? LayerMask.GetMask("Enemy") : LayerMask.GetMask("Player");
        contactFilter.SetLayerMask(mask);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayer)
        {
            laserActive = Input.GetMouseButton(0);
        }

        beamRenderer.enabled = laserActive;

        if (laserActive)
        {
            TryShoot();
        }
    }

    public void ToggleLaserBeam(bool toggle)
    {
        laserActive = toggle;
    }

    public override void TryShoot()
    {
        timeSinceLastTick += Time.deltaTime;
        if (timeSinceLastTick > laserTickTime)
        {
            RaycastHit2D[] hit = new RaycastHit2D[1];
            Physics2D.CircleCast(transform.position, laserBreadth, transform.up, contactFilter, hit, laserRange);
            if (hit[0])
            {
                IBulletHittable hitDetector = hit[0].transform.GetComponent<IBulletHittable>();
                if (hitDetector != null)
                {
                    hitDetector.OnBulletHit();
                }

                Vector2 direction = hit[0].transform.position - transform.position;
                float beamLength = direction.magnitude / transform.lossyScale.y;
                beamRenderer.size = new Vector2(0.8f, beamLength);
            }
            else
            {
                float beamLength = laserRange / transform.lossyScale.y;
                beamRenderer.size = new Vector2(0.8f, beamLength);
            }
            timeSinceLastTick = 0;
        }
    }

    new public void SetFireRate(float val)
    {
        base.SetFireRate(val);
        laserTickTime = 1f / effectiveFireRate;
    }

    new public void ResetFireRate()
    {
        base.ResetFireRate();
        laserTickTime = 1f / effectiveFireRate;
    }
}
