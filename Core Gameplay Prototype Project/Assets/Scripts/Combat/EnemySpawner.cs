using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    static Dictionary<EnemyFormationTiers, EnemyFormation[]> formationPools;
    // For use in boss levels -- spawn just one boss
    static GameObject[] bossPrefabs;

    public enum EnemyFormationTiers
    {
        Easy,
        Medium,
        Hard
    }

    EnemyFormation[] enemyFormations;
    float formationInterval;

    public static void LoadEnemyFormations()
    {
        if (formationPools != null && bossPrefabs != null)
        {
            return;
        }

        formationPools = new Dictionary<EnemyFormationTiers, EnemyFormation[]>();
        foreach(EnemyFormationTiers formationTier in System.Enum.GetValues(typeof(EnemyFormationTiers)))
        {
            string formationDirectory = "Scriptable Objects/Enemy Formations/" + formationTier.ToString();
            formationPools[formationTier] = Resources.LoadAll<EnemyFormation>(formationDirectory);
        }

        bossPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemies/Bosses");
    }

    private void Awake()
    {
        if (!CombatStageManager.Instance.isBossStage)
        {
            int level = GameManager.Instance.MapTier;

            List<EnemyFormation> formationPool = new List<EnemyFormation>();
            switch (level)
            {
                case 3:
                    formationInterval = 4.4f;
                    break;
                case 2:
                    formationInterval = 4.6f;
                    break;
                case 1:
                    formationInterval = 4.8f;
                    break;
                case 0:
                    formationInterval = 5f;
                    break;
            }

            enemyFormations = GameManager.Instance.GetStageFormations();
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

    public static EnemyFormation[] PrepareFormations(int level)
    {
        List<EnemyFormation> formationPool = new List<EnemyFormation>();
        switch (level)
        {
            case 3:
                formationPool.AddRange(formationPools[EnemyFormationTiers.Hard]);
                formationPool.AddRange(formationPools[EnemyFormationTiers.Medium]);
                break;
            case 2:
                formationPool.AddRange(formationPools[EnemyFormationTiers.Medium]);
                break;
            case 1:
                formationPool.AddRange(formationPools[EnemyFormationTiers.Medium]);
                formationPool.AddRange(formationPools[EnemyFormationTiers.Easy]);
                break;
            case 0:
                formationPool.AddRange(formationPools[EnemyFormationTiers.Easy]);
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

        return formationsDraw.ToArray();
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
        GameObject boss = Instantiate(bossPrefabs[GameManager.Instance.BossID]);
        boss.transform.position = new Vector3(0, 10, 0);

        EnemyBehavior bossBehavior = boss.GetComponent<EnemyBehavior>();
        Vector2[] bossPath = { new Vector2(0, 10), new Vector2(0, 3) };
        bossBehavior.SetPath(bossPath);
    }
}
