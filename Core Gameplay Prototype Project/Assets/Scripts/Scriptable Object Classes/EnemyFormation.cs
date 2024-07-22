using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Formation", menuName = "Enemy Formation Data")]
public class EnemyFormation : ScriptableObject
{
    [SerializeField] EnemyFormationMember[] enemies;

    public IEnumerator Deploy()
    {
        int i = 0;
        while (i < enemies.Length)
        {
            GameObject enemy = Instantiate(enemies[i].prefab);
            enemy.transform.position = enemies[i].curvePoints[0];
            EnemyBehavior enemyBehavior = enemy.GetComponent<EnemyBehavior>();
            enemyBehavior.SetPath(enemies[i].curvePoints);

            if (CombatStageManager.Instance != null)
            {
                CombatStageManager.Instance.RegisterEnemyTransform(enemy.transform);
            }

            i += 1;
            if (i < enemies.Length)
            {
                yield return new WaitForSeconds(enemies[i].spawnDelay);
            }
        }
    }

    public int GetEnemyCount()
    {
        return enemies.Length;
    }
}

[Serializable]
public class EnemyFormationMember
{
    public GameObject prefab;

    // Describes the time in seconds to wait since spawning the previous enemy.
    // Not applicable to first enemy in the formation.
    public float spawnDelay;

    // The first point and last points will serve as the enemy's
    // initial and final positions.
    public Vector2[] curvePoints;
}
