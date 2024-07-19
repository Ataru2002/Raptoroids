using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectilePoolManager : MonoBehaviour
{
    public static ProjectilePoolManager Instance
    {
        get; private set;
    }

    static GameObject[] playerProjectiles;
    static GameObject[] enemyProjectiles;

    Dictionary<PlayerProjectileType, LinkedPool<GameObject>> playerProjectilePools;
    Dictionary<EnemyProjectileType, LinkedPool<GameObject>> enemyProjectilePools;

    public static void LoadProjectiles()
    {
        playerProjectiles = Resources.LoadAll<GameObject>("Prefabs/Combat Objects/Projectiles/Pooled/Player");
        enemyProjectiles = Resources.LoadAll<GameObject>("Prefabs/Combat Objects/Projectiles/Pooled/Enemy");
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;

            playerProjectilePools = new Dictionary<PlayerProjectileType, LinkedPool<GameObject>>();
            Func<GameObject>[] playerSpawnFuncs =
            {
                MakePlayerBasicBullet,
                MakePlayerPairCurveProjectile,
                MakePlayerExplosiveProjectile,
                MakePlayerHomingProjectile
            };
            int playerProjectileTypeCount = Enum.GetValues(typeof(PlayerProjectileType)).Length;
            if (playerProjectileTypeCount != playerSpawnFuncs.Length)
            {
                Debug.LogError("Player projectile type count and spawn function count mismatch.");
            }
            for (int p = 0; p < playerSpawnFuncs.Length; p++)
            {
                PlayerProjectileType projectileType = (PlayerProjectileType)p;
                LinkedPool<GameObject> projectilePool = new LinkedPool<GameObject>(playerSpawnFuncs[p], PoolCommons.OnGetFromPool, PoolCommons.OnReleaseToPool, PoolCommons.OnPoolItemDestroy, false, 1000);
                playerProjectilePools[projectileType] = projectilePool;
            }

            enemyProjectilePools = new Dictionary<EnemyProjectileType, LinkedPool<GameObject>>();
            Func<GameObject>[] enemySpawnFuncs =
            {
                MakeEnemyBasicBullet
            };
            int enemyProjectileTypeCount = Enum.GetValues(typeof(EnemyProjectileType)).Length;
            if (enemyProjectileTypeCount != enemySpawnFuncs.Length)
            {
                Debug.LogError("Enemy projectile type count and spawn function count mismatch.");
            }
            for (int e = 0; e < enemySpawnFuncs.Length; e++)
            {
                EnemyProjectileType projectileType = (EnemyProjectileType)e;
                LinkedPool<GameObject> projectilePool = new LinkedPool<GameObject>(enemySpawnFuncs[e], PoolCommons.OnGetFromPool, PoolCommons.OnReleaseToPool, PoolCommons.OnPoolItemDestroy, false, 1000);
                enemyProjectilePools[projectileType] = projectilePool;
            }
        }
    }

    // Unity's LinkedPool constructor takes a Func<T>, where T is the return type (i.e., the type of the object to make).
    // This means a parameter cannot be passed to the function that instantiates the projectile.
    // Implementing a pool data structure that allows us to do so but functions just like Unity's LinkedPool
    // otherwise is likely not worth the effort.
    #region MAKE PLAYER PROJECTILE
    GameObject MakePlayerBasicBullet()
    {
        return Instantiate(playerProjectiles[(int)PlayerProjectileType.Basic]);
    }
    
    GameObject MakePlayerPairCurveProjectile()
    {
        return Instantiate(playerProjectiles[(int)PlayerProjectileType.CurvePair]);
    }

    GameObject MakePlayerExplosiveProjectile()
    {
        return Instantiate(playerProjectiles[(int)PlayerProjectileType.Explosive]);
    }

    GameObject MakePlayerHomingProjectile()
    {
        return Instantiate(playerProjectiles[(int)(PlayerProjectileType.Homing)]);
    }
    #endregion

    #region MAKE ENEMY PROJECTILE
    GameObject MakeEnemyBasicBullet()
    {
        return Instantiate(enemyProjectiles[(int)EnemyProjectileType.Basic]);
    }
    #endregion

    public GameObject GetPlayerProjectile(int typeID)
    {
        return playerProjectilePools[(PlayerProjectileType)typeID].Get();
    }

    public GameObject GetEnemyProjectile(int typeID)
    {
        return enemyProjectilePools[(EnemyProjectileType)typeID].Get();
    }

    public void ReturnPlayerProjectile(GameObject bullet)
    {
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        if (bulletComponent == null)
        {
            Debug.LogError("Returning an object that is not a projectile into a player projectile pool. Abort");
            return;
        }
        if (bulletComponent.ReleasedToPool)
        {
            Debug.LogError("Attempting to release a projectile that is already released. Abort");
            return;
        }

        int id = bulletComponent.GetBulletPoolID();
        playerProjectilePools[(PlayerProjectileType)id].Release(bullet);
    }

    public void ReturnEnemyProjectile(GameObject bullet)
    {
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        if (bulletComponent == null)
        {
            Debug.LogError("Returning an object that is not a projectile into an enemy projectile pool. Abort");
            return;
        }

        int id = bulletComponent.GetBulletPoolID();
        enemyProjectilePools[(EnemyProjectileType)id].Release(bullet);
    }
}

public enum PlayerProjectileType
{
    Basic,
    CurvePair,
    Explosive,
    Homing
}

public enum EnemyProjectileType
{
    Basic
}
