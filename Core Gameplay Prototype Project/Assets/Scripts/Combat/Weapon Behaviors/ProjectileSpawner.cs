using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ProjectileSpawner : Weapon
{
    [SerializeField] float firstShotDelay;
    [SerializeField] ShotType shotType;

    [SerializeField] int projectilesPerShot;

    [SerializeField] float coneAngle;

    [SerializeField] AudioSource sfxSource;

    EnemyBehavior enemyBehavior = null;

    delegate GameObject ProjectileSpawnFunc();
    ProjectileSpawnFunc spawnProjectile;

    Action shoot;

    float timestamp;
    float timeOfNextShot = 0;

    void Awake()
    {
        SetShootFunc();
        SetProjectileSpawnFunc();
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        ResetShotTimer();
    }

    // Update is called once per frame
    void Update()
    {
        timestamp = Time.time;

        if (isPlayer && Input.GetMouseButton(0))
        {
            TryShoot();
        }
    }

    void SetShootFunc()
    {
        switch(shotType)
        {
            case ShotType.Single:
                shoot = SingleShot; break;
            case ShotType.Cone:
                shoot = ConeShot; break;
            default:
                Debug.LogWarning("Setting shot to unsupported type. Falling back to single shot.");
                shoot = SingleShot;
                shotType = ShotType.Single;
                break;
        }
    }

    void SetProjectileSpawnFunc()
    {
        if (isPlayer)
        {
            if(CombatStageManager.Instance != null){
                spawnProjectile = CombatStageManager.Instance.GetPlayerProjectile;
            }
            else if (TutorialRoomManager.Instance != null) {
                spawnProjectile = TutorialRoomManager.Instance.GetPlayerProjectile;
            }
            else
            {
                enabled = false;
            }
        }
        else
        {
            enemyBehavior = GetComponentInParent<EnemyBehavior>();
            if(CombatStageManager.Instance != null){
                spawnProjectile = CombatStageManager.Instance.GetEnemyProjectile;
            }
            else{
                spawnProjectile = TutorialRoomManager.Instance.GetEnemyProjectile;
            }
        }
    }

    public void AssociateWeaponData(WeaponData weaponData)
    {
        shotType = weaponData.shotType;
        SetFireRate(weaponData.fireRate);

        if (shotType == ShotType.Cone)
        {
            coneAngle = weaponData.coneAngle;
            projectilesPerShot = weaponData.projectileCount;
        }

        SetShootFunc();
    }

    public void ResetShotTimer()
    {
        timeOfNextShot = firstShotDelay;
    }

    public override void TryShoot()
    {
        // Only let the enemy begin shooting once it gets in position.
        if (enemyBehavior != null && !enemyBehavior.FinalPositionReached)
        {
            return;
        }

        if (timestamp < timeOfNextShot)
        {
            return;
        }

        shoot();

        timeOfNextShot = Time.time + 1f / effectiveFireRate;
    }

    GameObject BaseProjectile()
    {
        GameObject projectile = spawnProjectile();
        projectile.transform.position = transform.position;
        projectile.transform.rotation = transform.rotation;

        // Rotate another 270 to match the fact that enemies are rotated
        if (!isPlayer)
        {
            projectile.transform.Rotate(0, 0, 270);
        }

        return projectile;
    }

    // --- Shot Pattern Functions ---
    // These will be referenced via the shoot action.
    void SingleShot()
    {
        BaseProjectile();
        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(sfxSource.clip);
        }
    }

    void ConeShot()
    {
        if (projectilesPerShot < 1)
        {
            throw new System.Exception("Projectile count is non-positive.");
        }

        if (projectilesPerShot % 2 == 0)
        {
            throw new System.Exception("Projectile count is even - there cannot be one single 'center' projectile");
        }

        GameObject centerProjectile = BaseProjectile();

        int projectileGroupSize = projectilesPerShot / 2;
        float angleMultiplier = 1f / projectileGroupSize * 0.5f;

        for (int i = 1; i <= projectileGroupSize; i++)
        {
            GameObject projectileA = spawnProjectile();
            GameObject projectileB = spawnProjectile();

            projectileA.transform.position = transform.position;
            projectileB.transform.position = transform.position;

            projectileA.transform.rotation = centerProjectile.transform.rotation;
            projectileB.transform.rotation = centerProjectile.transform.rotation;

            projectileA.transform.Rotate(0, 0, -angleMultiplier * i * coneAngle);
            projectileB.transform.Rotate(0, 0, angleMultiplier * i * coneAngle);
        }

        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(sfxSource.clip);
        }
    }
    // ---
}
