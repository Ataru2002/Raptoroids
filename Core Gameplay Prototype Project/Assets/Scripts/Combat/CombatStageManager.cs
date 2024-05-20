using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using GameAnalyticsSDK;
using UnityEngineInternal;
using Unity.Burst.CompilerServices;

public class CombatStageManager : MonoBehaviour
{
    static CombatStageManager instance;
    public static CombatStageManager Instance { get { return instance; } }

    [SerializeField] Transform playerSpawnPoint;
    GameObject[] playerPrefabs;

    WeaponData[] weaponDataBank;

    [SerializeField] float enemyRespawnDelay;

    [SerializeField] GameObject stageUI;
    [SerializeField] GameObject winScreen;
    [SerializeField] RectTransform winScreenSummaryBox;
    [SerializeField] GameObject bossWinScreen;
    [SerializeField] GameObject loseScreen;
    [SerializeField] RectTransform loseScreenSummaryBox;
    [SerializeField] TextMeshProUGUI killCounter;
    [SerializeField] GameObject bossHealthBar;
    [SerializeField] RectTransform bossHealthBarRect;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI hiScoreText;

    [SerializeField] GameObject runEndScoreCanvas;
    [SerializeField] TextMeshProUGUI runEndScoreText;
    [SerializeField] GameObject newHiScoreNotice;
    EnemySpawner enemySpawner;

    const float bossHealthBarWidth = 400f;
    const float bossHealthBarHeight = 50f;

    const float xBounds = 4f;
    const float yBounds = 7f;
    public float HorizontalLowerBound { get { return -xBounds; } }
    public float HorizontalUpperBound {  get { return xBounds; } }
    public float VerticalLowerBound { get {  return -yBounds; } }
    public float VerticalUpperBound { get {  return yBounds; } }

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

    [SerializeField] GameObject oakNutPrefab;
    LinkedPool<GameObject> oakNuts;

    [SerializeField] GameObject hillPrefab;
    LinkedPool<GameObject> hills;

    GameObject rewardSummaryPrefab;

    GameObject playerObject;
    public Transform PlayerTransform { get { return playerObject.transform; } }

    public bool isBossStage { get { return GameManager.Instance.MapTier >= 4; } }
    bool stageEnded = false;

    [SerializeField] GameObject bossHintCanvas;
    [SerializeField] TextMeshProUGUI hint;
    [SerializeField] Image warningSign;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;

            playerProjectilePrefab = Resources.Load<GameObject>("Prefabs/Combat Objects/PlayerBullet");
            enemyProjectilePrefab = Resources.Load<GameObject>("Prefabs/Combat Objects/EnemyBullet");
            hitParticlesPrefab = Resources.Load<GameObject>("Prefabs/Combat Objects/EnemyImpactParticles");
            rewardSummaryPrefab = Resources.Load<GameObject>("Prefabs/UI Elements/StageSummaryItem");
            playerPrefabs = Resources.LoadAll<GameObject>("Prefabs/Raptoroids/Armed");
            weaponDataBank = Resources.LoadAll<WeaponData>("Scriptable Objects/Weapons");
        }
    }

    const float oakNutFallSpeed = 2f;
    const float hillSpeed = 2f;
    const float playerFreezeDuration = 5f;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.LoadingDataStart();

        enemySpawner = GetComponent<EnemySpawner>();
        enemyKillRequirement = enemySpawner.GetEnemyCount();

        int raptoroidID = PlayerPrefs.HasKey("EquippedRaptoroid") ? PlayerPrefs.GetInt("EquippedRaptoroid") : 0;
        playerObject = Instantiate(playerPrefabs[raptoroidID]);
        playerObject.transform.position = playerSpawnPoint.position;

        int gunID = PlayerPrefs.HasKey("EquippedWeapon") ? PlayerPrefs.GetInt("EquippedWeapon") : 0;
        playerObject.GetComponentInChildren<ProjectileSpawner>().AssociateWeaponData(weaponDataBank[gunID]);

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

        oakNuts = new LinkedPool<GameObject>(MakeOakNut, OnGetFromPool, OnReleaseToPool, OnPoolItemDestroy, false, 100);
        StartCoroutine(StartOakNutSpawn());

        hills = new LinkedPool<GameObject>(MakeHill, OnGetFromPool, OnReleaseToPool, OnPoolItemDestroy, false, 100);
        StartCoroutine(StartHillSpawn());
        UpdateScoreDisplay();
    }

    GameObject MakeOakNut()
    {
        GameObject oakNut = Instantiate(oakNutPrefab);
        oakNut.SetActive(false); // Set inactive initially
        return oakNut;
    }

    IEnumerator StartOakNutSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // Spawn oak nut every 20 seconds
            SpawnOakNut();
        }
    }

    void SpawnOakNut()
    {
        GameObject oakNut = oakNuts.Get();
        oakNut.transform.position = new Vector3(Random.Range(-5f, 5f), 10f, 0f); // Randomize spawn position
        oakNut.SetActive(true);
    }

    GameObject MakeHill()
    {
        GameObject hill = Instantiate(hillPrefab);
        hill.SetActive(false); // Set inactive initially
        return hill;
    }

    IEnumerator StartHillSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // Spawn hill every 20 seconds
            SpawnHill();
        }
    }

    void SpawnHill()
    {
        GameObject hill = hills.Get();
        hill.transform.position = new Vector3(Random.Range(-5f, 5f), 10f, 0f); // Randomize spawn position
        hill.SetActive(true);
    }

    void OnDisable()
    {
        StopAllCoroutines(); // Stop spawning coroutine when the stage ends
    }

    #region Pool Functions
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
    #endregion

    public void OnGemSpawn()
    {
        gemsWaiting += 1;
    }

    public void OnGemPickup(int gemValue)
    {
        gemsCollectedInStage += gemValue;
        gemsWaiting -= 1;
        GameManager.Instance.UpdateGemSourceData(GemSources.Combat, gemValue);
    }

    public void OnGemDespawn()
    {
        gemsWaiting -= 1;
    }

    public void UpdateBossHealthBar(float healthRatio)
    {
        bossHealthBarRect.sizeDelta = new Vector2(bossHealthBarWidth * healthRatio, bossHealthBarHeight);
    }

    public void DisplayBossHint(string msg, float duration = 3f)
    {
        StartCoroutine(BossHintDisplaySequence(msg, duration));
    }

    IEnumerator BossHintDisplaySequence(string msg, float duration)
    {
        bossHintCanvas.SetActive(true);
        hint.text = msg;
        StartCoroutine(WarningSignFlash());
        yield return new WaitForSeconds(duration);
        StopCoroutine(WarningSignFlash());
        bossHintCanvas.SetActive(false);
    }

    IEnumerator WarningSignFlash()
    {
        bool signOn = true;
        while (true)
        {
            warningSign.enabled = signOn;
            yield return new WaitForSeconds(0.3f);
            signOn = !signOn;
        }
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

    public void UpdateScore(int score)
    {
        GameManager.Instance.AddScore(score);
        UpdateScoreDisplay();
    }

    public void UpdateScoreDisplay()
    {
        scoreText.text = GameManager.Instance.GetCurrentScore().ToString();
        hiScoreText.text = GameManager.Instance.GetHighScore().ToString();
    }

    void EndStage(bool playerWin)
    {
        if (stageEnded)
        {
            return;
        }

        stageEnded = true;
        stageUI.SetActive(false);
        GameManager.Instance.updateProgress("Quest 2", enemyKillCount);

        if (playerWin)
        {
            StartCoroutine(LevelEndGemCollectChance());
        }
        else
        {
            loseScreen.SetActive(true);
            runEndScoreCanvas.SetActive(true);
            runEndScoreText.text = GameManager.Instance.GetCurrentScore().ToString();
            newHiScoreNotice.SetActive(GameManager.Instance.HighScoreChanged);
            Time.timeScale = 0;

            int grossGems = GameManager.Instance.GetCurrentGems();

            GameObject collectedGemCounter = Instantiate(rewardSummaryPrefab);
            collectedGemCounter.transform.SetParent(loseScreenSummaryBox);
            collectedGemCounter.transform.localScale = Vector3.one;
            string grossGemText = $"Gems collected this run: {grossGems}";
            LocalizeStringEvent grossGemEvent = collectedGemCounter.GetComponentInChildren<LocalizeStringEvent>();
            grossGemEvent.StringReference.Add("runGems", new IntVariable { Value = grossGems });
            grossGemEvent.SetTable("StageClearScreen");
            grossGemEvent.SetEntry("RunGems");

            GameObject failureMarker = Instantiate(rewardSummaryPrefab);
            failureMarker.transform.SetParent(loseScreenSummaryBox);
            failureMarker.transform.localScale = Vector3.one;
            LocalizeStringEvent penaltyEvent = failureMarker.GetComponentInChildren<LocalizeStringEvent>();
            penaltyEvent.SetTable("LoseScreen");
            penaltyEvent.SetEntry("Penalty");

            GameObject finalGemCounter = Instantiate(rewardSummaryPrefab);
            finalGemCounter.transform.SetParent(loseScreenSummaryBox);
            finalGemCounter.transform.localScale = Vector3.one;
            LocalizeStringEvent netGemEvent = finalGemCounter.GetComponentInChildren<LocalizeStringEvent>();
            netGemEvent.StringReference.Add("netGems", new IntVariable { Value = Mathf.CeilToInt(0.8f * grossGems) });
            netGemEvent.SetTable("LoseScreen");
            netGemEvent.SetEntry("FinalGemCount");

            GameManager.Instance.SetGemPenaltyData(Mathf.FloorToInt(0.2f * grossGems));
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
            runEndScoreCanvas.SetActive(true);
            runEndScoreText.text = GameManager.Instance.GetCurrentScore().ToString();
            newHiScoreNotice.SetActive(GameManager.Instance.HighScoreChanged);
        }
        else
        {
            winScreen.SetActive(true);

            GameManager.Instance.CollectGems(gemsCollectedInStage);

            GameObject stageGemsBox = Instantiate(rewardSummaryPrefab);
            stageGemsBox.transform.SetParent(winScreenSummaryBox);
            stageGemsBox.transform.localScale = Vector3.one;
            LocalizeStringEvent stageGemEvent = stageGemsBox.GetComponentInChildren<LocalizeStringEvent>();
            stageGemEvent.StringReference.Add("stageGems", new IntVariable { Value = gemsCollectedInStage });
            stageGemEvent.SetTable("StageClearScreen");
            stageGemEvent.SetEntry("StageGems");

            int runTotalGems = GameManager.Instance.GetCurrentGems();
            GameObject runTotalBox = Instantiate(rewardSummaryPrefab);
            runTotalBox.transform.SetParent(winScreenSummaryBox);
            runTotalBox.transform.localScale = Vector3.one;
            LocalizeStringEvent totalGemEvent = runTotalBox.GetComponentInChildren<LocalizeStringEvent>();
            totalGemEvent.StringReference.Add("runGems", new IntVariable { Value = runTotalGems });
            totalGemEvent.SetTable("StageClearScreen");
            totalGemEvent.SetEntry("RunGems");
        }
    }

    public void AdvanceRun()
    {
        GameManager.Instance.AdvanceMapProgress();
    }

    public void EndRun(bool playerWon)
    {
        if (playerWon)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Mission");
        }
        else
        {
            if (isBossStage)
            {
                // BossID is 0-indexed for array access purposes. Add 1 to match the ID number
                GameAnalytics.SetCustomDimension01("");
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "Mission", "Boss");
                GameAnalytics.SetCustomDimension01(null);
            }
            else
            {
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "Mission", "ActionStage", GameManager.Instance.MapTier);
            }
        }

        GameManager.Instance.ClearMapInfo();
        GameManager.Instance.ResetScore();

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
