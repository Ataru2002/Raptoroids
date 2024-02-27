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
    [SerializeField] GameObject bossWinScreen;
    [SerializeField] GameObject loseScreen;
    [SerializeField] TextMeshProUGUI killCounter;
    [SerializeField] GameObject bossHealthBar;
    [SerializeField] RectTransform bossHealthBarRect;

    EnemySpawner enemySpawner;

    const float bossHealthBarWidth = 500f;
    const float bossHealthBarHeight = 50f;

    int enemyKillCount = 0;

    public bool isBossStage { get { return GameManager.Instance.MapTier >= 4; } }

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

        if (isBossStage)
        {
            enemyKillRequirement = 1;
            killCounter.gameObject.SetActive(false);
            enemySpawner.SpawnBossEnemy();
        }
        else
        {
            bossHealthBar.SetActive(false);
            while (activeEnemies < MaxConcurrentEnemies)
            {
                enemySpawner.SpawnEnemy();
                activeEnemies++;
            }
        }

        killCounter.text = string.Format("0 / {0}", enemyKillRequirement);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateBossHealthBar(float healthRatio)
    {
        bossHealthBarRect.sizeDelta = new Vector2(bossHealthBarWidth * healthRatio, bossHealthBarHeight);
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
            if (isBossStage)
            {
                bossWinScreen.SetActive(true);
            }
            else
            {
                winScreen.SetActive(true);
            }
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

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
        GameManager.Instance.ClearMapInfo();
    }

    public void GoToMap()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MapSelection");
        GameManager.Instance.AdvanceMapProgress();
    }
}
