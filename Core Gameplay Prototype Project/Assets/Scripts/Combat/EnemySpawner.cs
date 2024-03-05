using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Vector2 finalPointLowerBound;
    [SerializeField] Vector2 finalPointUpperBound;

    // TODO: possibly get a list of enemies that can spawn in this level
    // once there is a sufficient variety of enemies in the game
    [SerializeField] GameObject[] enemyPrefabs;
    GameObject[] bossPrefabs;

    private void Awake()
    {
        bossPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemies/Bosses");
        print(bossPrefabs.Length);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemy()
    {
        int enemyIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject enemy = Instantiate(enemyPrefabs[enemyIndex]);
        
        // Bring the enemy far above the viewport to hide the enemy on spawn,
        // lest we get an enemy blip in a la FnaF 1 Golden Freddy.
        enemy.transform.position = new Vector3(0, 10, 0);

        float xPos = Random.Range(finalPointLowerBound.x, finalPointUpperBound.x);
        float yPos = Random.Range(finalPointLowerBound.y, finalPointUpperBound.y);

        EnemyBehavior enemyBehavior = enemy.GetComponent<EnemyBehavior>();
        enemyBehavior.SetInitialPosition(new Vector3(Random.Range(finalPointLowerBound.x, finalPointUpperBound.x), 7, 0));
        enemyBehavior.SetFinalPosition(new Vector3(xPos, yPos, 0));
    }

    // TODO: get data on what the boss will be from the Game Manager
    public void SpawnBossEnemy()
    {
        GameObject boss = Instantiate(bossPrefabs[0]);
        boss.transform.position = new Vector3(0, 10, 0);

        EnemyBehavior bossBehavior = boss.GetComponent<EnemyBehavior>();
        bossBehavior.SetInitialPosition(new Vector3(0, 10, 0));
        bossBehavior.SetFinalPosition(new Vector3(0, 3, 0));
    }
}
