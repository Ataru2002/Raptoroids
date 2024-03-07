using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    EnemyFormation[] enemyFormations;

    [SerializeField] Vector2 finalPointLowerBound;
    [SerializeField] Vector2 finalPointUpperBound;

    // TODO: possibly get a list of enemies that can spawn in this level
    // once there is a sufficient variety of enemies in the game
    [SerializeField] GameObject[] enemyPrefabs;

    // For use in boss levels -- spawn just one boss
    GameObject[] bossPrefabs;

    private void Awake()
    {
        bossPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemies/Bosses");
        enemyFormations = Resources.LoadAll<EnemyFormation>("Scriptable Objects/Enemy Formations");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeployFormation()
    {
        int formationIndex = Random.Range(0, enemyFormations.Length);
        StartCoroutine(enemyFormations[formationIndex].Deploy());
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
