using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.Presets;
using UnityEngine;

public class ProjectileSpawner : Weapon
{
    [SerializeField] float firstShotDelay;
    //[SerializeField] ShotType shotType;

    //[SerializeField] int projectilesPerShot;

    //[SerializeField] float coneAngle;

    [SerializeField] AudioSource sfxSource;

    EnemyBehavior enemyBehavior = null;

    delegate GameObject ProjectileSpawnFunc(int projectileID);
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

        if (weaponData.isPlayer && Input.GetMouseButton(0))
        {
            TryShoot();
        }
    }

    void SetShootFunc()
    {
        if (weaponData == null)
        {
            return;
        }

        switch(weaponData.shotType)
        {
            case ShotType.Single:
                shoot = SingleShot; break;
            case ShotType.Cone:
                shoot = ConeShot; break;
            default:
                Debug.LogWarning("Setting shot to unsupported type. Falling back to single shot.");
                shoot = SingleShot;
                break;
        }
    }

    void SetProjectileSpawnFunc()
    {
        if (weaponData == null)
        {
            return;
        }

        spawnProjectile = weaponData.isPlayer ? ProjectilePoolManager.Instance.GetPlayerProjectile : ProjectilePoolManager.Instance.GetEnemyProjectile;
    }

    new public void SetWeaponData(WeaponData weaponData)
    {
        base.SetWeaponData(weaponData);
        SetShootFunc();
        SetProjectileSpawnFunc();
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
        // TODO: get the ID value from elsewhere to correspond to the player's selected weapon
        GameObject projectile = spawnProjectile(0);
        projectile.transform.position = transform.position;
        projectile.transform.rotation = transform.rotation;

        // Rotate another 270 to match the fact that enemies are rotated
        if (!weaponData.isPlayer)
        {
            projectile.transform.Rotate(0, 0, 270);
        }

        return projectile;
    }

    #region Shot Patterns
    // --- Shot Pattern Functions ---
    // These will be referenced via the shoot action.
    void SingleShot()
    {
        BaseProjectile();
        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(weaponData.shotSound);
        }
    }

    void ConeShot()
    {
        if (weaponData.projectileCount < 1)
        {
            throw new System.Exception("Projectile count is non-positive.");
        }

        if (weaponData.projectileCount % 2 == 0)
        {
            throw new System.Exception("Projectile count is even - there cannot be one single 'center' projectile");
        }

        GameObject centerProjectile = BaseProjectile();

        int projectileGroupSize = weaponData.projectileCount / 2;
        float angleMultiplier = 1f / projectileGroupSize * 0.5f;

        for (int i = 1; i <= projectileGroupSize; i++)
        {
            // TODO: get the ID value from elsewhere to correspond to the player's selected weapon
            GameObject projectileA = spawnProjectile(0);
            GameObject projectileB = spawnProjectile(0);

            projectileA.transform.position = transform.position;
            projectileB.transform.position = transform.position;

            projectileA.transform.rotation = centerProjectile.transform.rotation;
            projectileB.transform.rotation = centerProjectile.transform.rotation;

            projectileA.transform.Rotate(0, 0, -angleMultiplier * i * weaponData.coneAngle);
            projectileB.transform.Rotate(0, 0, angleMultiplier * i * weaponData.coneAngle);
        }

        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(weaponData.shotSound);
        }
    }
    // ---
    #endregion
}
