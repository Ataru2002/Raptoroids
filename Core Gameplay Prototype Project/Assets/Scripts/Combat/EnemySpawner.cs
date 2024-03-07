using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    static EnemyFormation[] easyFormations;
    static EnemyFormation[] mediumFormations;
    static EnemyFormation[] hardFormations;

    EnemyFormation[] enemyFormations;
    float formationInterval;

    // For use in boss levels -- spawn just one boss
    GameObject[] bossPrefabs;

    private void Awake()
    {
        if (easyFormations == null)
        {
            easyFormations = Resources.LoadAll<EnemyFormation>("Scriptable Objects/Enemy Formations/Easy");
        }

        if (mediumFormations == null)
        {
            mediumFormations = Resources.LoadAll<EnemyFormation>("Scriptable Objects/Enemy Formations/Medium");
        }

        if (hardFormations == null)
        {
            hardFormations = Resources.LoadAll<EnemyFormation>("Scriptable Objects/Enemy Formations/Hard");
        }

        bossPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemies/Bosses");

        if (!CombatStageManager.Instance.isBossStage)
        {
            int level = GameManager.Instance.MapTier;

            // TODO: once more enemy formations are made, phase out the lower-difficulty formations
            // from each level's pool
            List<EnemyFormation> formationPool = new List<EnemyFormation>();
            switch (level)
            {
                case 3:
                    formationPool.AddRange(hardFormations);
                    formationPool.AddRange(mediumFormations);
                    formationPool.AddRange(easyFormations);

                    formationInterval = 1.8f;
                    break;
                case 2:
                    formationPool.AddRange(mediumFormations);
                    formationPool.AddRange(easyFormations);

                    formationInterval = 2.2f;
                    break;
                case 1:
                    formationPool.AddRange(mediumFormations);
                    formationPool.AddRange(easyFormations);

                    formationInterval = 2.6f;
                    break;
                case 0:
                    formationPool.AddRange(easyFormations);

                    formationInterval = 3f;
                    break;
            }

            for (int i = formationPool.Count - 1; i > 0; i--)
            {
                int targetIndex = Random.Range(0, i + 1);
                EnemyFormation temp = formationPool[targetIndex];
                formationPool[targetIndex] = formationPool[i];
                formationPool[i] = temp;
            }

            List<EnemyFormation> formationsDraw = formationPool.GetRange(0, level + 2);

            enemyFormations = formationsDraw.ToArray();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDeployInterval(float deployInterval)
    {
        formationInterval = deployInterval;
    }

    public int GetEnemyCount()
    {
        if (CombatStageManager.Instance.isBossStage)
        {
            return 1;
        }

        int sum = 0;
        for (int i = 0; i < enemyFormations.Length; i++)
        {
            sum += enemyFormations[i].GetEnemyCount();
        }
        return sum;
    }

    public IEnumerator DeployFormations()
    {
        for (int i  = 0; i < enemyFormations.Length; i++)
        {
            StartCoroutine(enemyFormations[i].Deploy());
            yield return new WaitForSeconds(formationInterval);
        }
    }

    // TODO: get data on what the boss will be from the Game Manager
    public void SpawnBossEnemy()
    {
        GameObject boss = Instantiate(bossPrefabs[0]);
        boss.transform.position = new Vector3(0, 10, 0);

        EnemyBehavior bossBehavior = boss.GetComponent<EnemyBehavior>();
        Vector2[] bossPath = { new Vector2(0, 10), new Vector2(0, 3) };
        bossBehavior.SetPath(bossPath);
    }
}
