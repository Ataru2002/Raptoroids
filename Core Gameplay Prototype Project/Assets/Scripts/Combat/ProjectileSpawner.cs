using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] bool isPlayer;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float fireRate;
    [SerializeField] bool coneFire;
    [SerializeField] float coneAngle;

    // Randomizing shot times will use the fire rate to determine the
    // middle point of the range.
    [SerializeField] bool randomizeShotTimes;
    [SerializeField] float shotTimeMaxDelta;

    float shotInterval;
    float timeOfNextShot = 0;

    // Start is called before the first frame update
    void Start()
    {
        shotInterval = 1f / fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        float timestamp = Time.time;

        if (isPlayer && !Input.GetMouseButton(0))
        {
            return;
        }

        if (timestamp >= timeOfNextShot)
        {
            GameObject projectile = Instantiate(projectilePrefab);
            projectile.transform.position = transform.position;
            projectile.transform.rotation = transform.rotation;
            
            // Rotate another 270 to match the fact that enemies are rotated
            if (!isPlayer)
            {
                projectile.transform.Rotate(0, 0, 270);
            }

            if (coneFire)
            {
                GameObject projectile2 = Instantiate(projectilePrefab);
                GameObject projectile3 = Instantiate(projectilePrefab);

                projectile2.transform.position = transform.position;
                projectile3.transform.position = transform.position;

                projectile2.transform.rotation = projectile.transform.rotation;
                projectile3.transform.rotation = projectile.transform.rotation;

                projectile2.transform.Rotate(0, 0, -0.5f * coneAngle);
                projectile3.transform.Rotate(0, 0, 0.5f * coneAngle);
            }

            timeOfNextShot = Time.time + shotInterval;
            if (randomizeShotTimes)
            {
                timeOfNextShot += Random.Range(-shotTimeMaxDelta, shotTimeMaxDelta);
            }
        }
    }
}
