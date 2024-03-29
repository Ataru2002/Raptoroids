using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using TMPro;

public class CombatStageManager : MonoBehaviour
{
    static CombatStageManager instance;
    public static CombatStageManager Instance { get { return instance; } }

    [SerializeField] Transform playerSpawnPoint;
    [SerializeField] GameObject[] playerPrefabs;

    [SerializeField] float enemyRespawnDelay;
    [SerializeField] int maxConcurrentEnemies;

    public int MaxConcurrentEnemies { get {  return maxConcurrentEnemies; } }

    [SerializeField] GameObject stageUI;
    [SerializeField] GameObject winScreen;
    [SerializeField] RectTransform winScreenSummaryBox;
    [SerializeField] GameObject bossWinScreen;
    [SerializeField] GameObject loseScreen;
    [SerializeField] RectTransform loseScreenSummaryBox;
    [SerializeField] TextMeshProUGUI killCounter;
    [SerializeField] GameObject bossHealthBar;
    [SerializeField] RectTransform bossHealthBarRect;

    EnemySpawner enemySpawner;

    const float bossHealthBarWidth = 500f;
    const float bossHealthBarHeight = 50f;

    int enemyKillRequirement;
    int enemyKillCount = 0;
    
    int gemsCollectedInStage = 0;

    int gemsWaiting = 0;

    LinkedPool<GameObject> playerProjectiles;
    GameObject playerProjectilePrefab;

    LinkedPool<GameObject> enemyProjectiles;
    GameObject enemyProjectilePrefab;

    LinkedPool<GameObject> enemyHitParticles;
    GameObject hitParticlesPrefab;

    GameObject rewardSummaryPrefab;

    public bool isBossStage { get { return GameManager.Instance.MapTier >= 4; } }
    bool stageEnded = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;

            playerProjectilePrefab = Resources.Load<GameObject>("Prefabs/Combat Objects/PlayerBullet");
            enemyProjectilePrefab = Resources.Load<GameObject>("Prefabs/Combat Objects/EnemyBullet");
            hitParticlesPrefab = Resources.Load<GameObject>("Prefabs/Combat Objects/EnemyImpactParticles");
            rewardSummaryPrefab = Resources.Load<GameObject>("Prefabs/UI Elements/StageSummaryItem");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        enemySpawner = GetComponent<EnemySpawner>();
        enemyKillRequirement = enemySpawner.GetEnemyCount();

        int raptoroidID = PlayerPrefs.HasKey("EquippedRaptoroid") ? PlayerPrefs.GetInt("EquippedRaptoroid") : 0;
        GameObject player = Instantiate(playerPrefabs[raptoroidID]);
        player.transform.position = playerSpawnPoint.position;

        if (isBossStage)
        {
            enemyKillRequirement = 1;
            killCounter.gameObject.SetActive(false);
            enemySpawner.SpawnBossEnemy();
        }
        else
        {
            bossHealthBar.SetActive(false);
            StartCoroutine(enemySpawner.DeployFormations());
        }

        playerProjectiles = new LinkedPool<GameObject>(MakePlayerProjectile, OnGetFromPool, OnReleaseToPool, OnPoolItemDestroy, false, 1000);
        enemyProjectiles = new LinkedPool<GameObject>(MakeEnemyProjectile, OnGetFromPool, OnReleaseToPool, OnPoolItemDestroy, false, 1000);
        enemyHitParticles = new LinkedPool<GameObject>(MakeEnemyParticles, OnGetFromPool, OnReleaseToPool, OnPoolItemDestroy, false, 1000);

        killCounter.text = string.Format("0 / {0}", enemyKillRequirement);
    }

    // Pooling functions
    GameObject MakePlayerProjectile()
    {
        return Instantiate(playerProjectilePrefab);
    }

    public GameObject GetPlayerProjectile()
    {
        return playerProjectiles.Get();
    }

    public void ReturnPlayerProjectile(GameObject target)
    {
        playerProjectiles.Release(target);
    }

    GameObject MakeEnemyProjectile()
    {
        return Instantiate(enemyProjectilePrefab);
    }

    public GameObject GetEnemyProjectile()
    {
        return enemyProjectiles.Get();
    }

    public void ReturnEnemyProjectile(GameObject target)
    {
        enemyProjectiles.Release(target);
    }

    GameObject MakeEnemyParticles()
    {
        return Instantiate(hitParticlesPrefab);
    }

    public GameObject GetEnemyHitParticles()
    {
        return enemyHitParticles.Get();
    }

    public void ReturnEnemyParticles(GameObject target)
    {
        enemyHitParticles.Release(target);
    }

    void OnGetFromPool(GameObject item)
    {
        item.SetActive(true);
    }

    void OnReleaseToPool(GameObject item)
    {
        item.SetActive(false);
    }

    private void OnPoolItemDestroy(GameObject item)
    {
        Destroy(item);
    }

    // End of Pooling functions

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGemSpawn()
    {
        gemsWaiting += 1;
    }

    public void OnGemPickup(int gemValue)
    {
        gemsCollectedInStage += gemValue;
        gemsWaiting -= 1;
    }

    public void OnGemDespawn()
    {
        gemsWaiting -= 1;
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
    }

    void EndStage(bool playerWin)
    {
        if (stageEnded)
        {
            return;
        }

        stageEnded = true;

        if (playerWin)
        {
            StartCoroutine(LevelEndGemCollectChance());
        }
        else
        {
            loseScreen.SetActive(true);
            Time.timeScale = 0;

            int grossGems = GameManager.Instance.GetCurrentGems();

            GameObject collectedGemCounter = Instantiate(rewardSummaryPrefab);
            collectedGemCounter.transform.SetParent(loseScreenSummaryBox);
            collectedGemCounter.transform.localScale = Vector3.one;
            string grossGemText = $"Gems collected this run: {grossGems}";
            collectedGemCounter.GetComponentInChildren<TextMeshProUGUI>().text = grossGemText;

            GameObject failureMarker = Instantiate(rewardSummaryPrefab);
            failureMarker.transform.SetParent(loseScreenSummaryBox);
            failureMarker.transform.localScale = Vector3.one;
            failureMarker.GetComponentInChildren<TextMeshProUGUI>().text = "Failure penalty: 20% reduction";

            GameObject finalGemCounter = Instantiate(rewardSummaryPrefab);
            finalGemCounter.transform.SetParent(loseScreenSummaryBox);
            finalGemCounter.transform.localScale = Vector3.one;
            string netGemText = $"Final gem amount awarded: {Mathf.CeilToInt(0.8f * grossGems)}";
            finalGemCounter.GetComponentInChildren<TextMeshProUGUI>().text = netGemText;
        }
    }

    IEnumerator LevelEndGemCollectChance()
    {
        while (gemsWaiting > 0)
        {
            yield return new WaitForEndOfFrame();
        }

        Time.timeScale = 0;

        if (isBossStage)
        {
            bossWinScreen.SetActive(true);
        }
        else
        {
            winScreen.SetActive(true);

            GameManager.Instance.CollectGems(gemsCollectedInStage);

            GameObject stageGemsBox = Instantiate(rewardSummaryPrefab);
            stageGemsBox.transform.SetParent(winScreenSummaryBox);
            stageGemsBox.transform.localScale = Vector3.one;
            string stageGemText = $"Gems collected this stage: {gemsCollectedInStage}";
            stageGemsBox.GetComponentInChildren<TextMeshProUGUI>().text = stageGemText;

            int runTotalGems = GameManager.Instance.GetCurrentGems();
            GameObject runTotalBox = Instantiate(rewardSummaryPrefab);
            runTotalBox.transform.SetParent(winScreenSummaryBox);
            runTotalBox.transform.localScale = Vector3.one;
            string totalGemText = $"Total gems collected this run: {runTotalGems}";
            runTotalBox.GetComponentInChildren<TextMeshProUGUI>().text = totalGemText;
        }
    }

    // For prototype use ONLY. Allows player to play again
    public void ReloadStage()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AdvanceRun()
    {
        GameManager.Instance.AdvanceMapProgress();
    }

    public void EndRun(bool playerWon)
    {
        GameManager.Instance.ClearMapInfo();

        // TODO: possibly handle multipliers in Game Manager instead
        float multiplier = playerWon ? 1f : 0.8f;
        GameManager.Instance.CommitCollectedGems(multiplier);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToMap()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MapSelection");
    }
}
