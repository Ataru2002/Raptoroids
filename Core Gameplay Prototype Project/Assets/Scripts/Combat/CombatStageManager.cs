using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CombatStageManager : MonoBehaviour
{
    static CombatStageManager instance;
    public static CombatStageManager Instance { get { return instance; } }

    [SerializeField] float enemyRespawnDelay;
    [SerializeField] int maxConcurrentEnemies;
    int activeEnemies = 0;
    public int MaxConcurrentEnemies { get {  return maxConcurrentEnemies; } }
    [SerializeField] int enemyKillRequirement;

    [SerializeField] GameObject stageUI;
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject loseScreen;
    [SerializeField] TextMeshProUGUI killCounter;

    EnemySpawner enemySpawner;

    int enemyKillCount = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        enemySpawner = GetComponent<EnemySpawner>();
        while (activeEnemies < MaxConcurrentEnemies)
        {
            enemySpawner.SpawnEnemy();
            activeEnemies++;
        }

        killCounter.text = string.Format("0 / {0}", enemyKillRequirement);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayerDefeated()
    {
        EndStage(false);
    }

    public void OnEnemyDefeated()
    {
        killCounter.text = string.Format("{0} / {1}", ++enemyKillCount, enemyKillRequirement);

        if (enemyKillCount >= enemyKillRequirement)
        {
            EndStage(true);
        }
        else
        {
            StartCoroutine(RespawnEnemy());
        }
    }

    IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(enemyRespawnDelay);
        enemySpawner.SpawnEnemy();
    }

    void EndStage(bool playerWin)
    {
        Time.timeScale = 0;

        if (playerWin)
        {
            winScreen.SetActive(true);
        }
        else
        {
            loseScreen.SetActive(true);
        }

    }

    // For prototype use ONLY. Allows player to play again
    public void ReloadStage()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}