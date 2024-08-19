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

public class CombatStageManager : MonoBehaviour
{
    static CombatStageManager instance;
    public static CombatStageManager Instance { get { return instance; } }

    [SerializeField] Transform playerSpawnPoint;
    GameObject[] playerPrefabs;

    WeaponData[] playerWeaponDataBank;

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
    [SerializeField] GameObject joystickPrefab;
    GameObject joystick;
    [SerializeField] GameObject OaknutHitScreen;
    [SerializeField] CombatBGMSelector bgmSelector;

    [SerializeField] GameObject runEndScoreCanvas;
    [SerializeField] TextMeshProUGUI runEndScoreText;
    [SerializeField] GameObject newHiScoreNotice;

    [SerializeField] GameObject tutorialEndScreen;
    [SerializeField] LocalizeStringEvent tutorialEndTextLocalizeEvent;

    EnemySpawner enemySpawner;

    const float bossHealthBarWidth = 370f;
    const float bossHealthBarHeight = 50f;

    #region Level Bounds
    const float xBounds = 4f;
    const float yBounds = 7f;
    public float HorizontalLowerBound { get { return -xBounds; } }
    public float HorizontalUpperBound {  get { return xBounds; } }
    public float VerticalLowerBound { get {  return -yBounds; } }
    public float VerticalUpperBound { get {  return yBounds; } }
    #endregion

    int enemyKillRequirement;
    int enemyKillCount = 0;

    int gemsCollectedInStage = 0;

    int gemsWaiting = 0;

    LinkedPool<GameObject> enemyHitParticles;
    GameObject hitParticlesPrefab;

    [SerializeField] GameObject oakNutPrefab;
    LinkedPool<GameObject> oakNuts;

    [SerializeField] GameObject minePrefab;
    LinkedPool<GameObject> mines;

    [SerializeField] GameObject hillPrefab;
    LinkedPool<GameObject> hills;

    GameObject rewardSummaryPrefab;

    GameObject playerObject;
    public Transform PlayerTransform { get { return playerObject.transform; } }

    List<Transform> enemyTransforms;

    public bool isBossStage { get { return GameManager.Instance.MapTier >= 4; } }
    public bool stageEnded { get; private set; } = false;

    [SerializeField] GameObject bossHintCanvas;
    [SerializeField] LocalizeStringEvent hint;
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

            hitParticlesPrefab = Resources.Load<GameObject>("Prefabs/Combat Objects/EnemyImpactParticles");
            rewardSummaryPrefab = Resources.Load<GameObject>("Prefabs/UI Elements/StageSummaryItem");
            playerPrefabs = Resources.LoadAll<GameObject>("Prefabs/Raptoroids");
            playerWeaponDataBank = Resources.LoadAll<WeaponData>("Scriptable Objects/Weapons/Player");

            // No combat stage will have more than 32 enemies (as it is now)
            // so allocate the transform list with capacity 32 to avoid reallocation
            // when adding transforms to the list
            enemyTransforms = new List<Transform>(32);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.LoadingDataStart("Quest 2");

        enemySpawner = GetComponent<EnemySpawner>();
        enemyKillRequirement = enemySpawner.GetEnemyCount();

        int raptoroidID = GameManager.Instance.tutorialMode ? 0 : GameManager.Instance.EquippedRaptoroid;
        playerObject = Instantiate(playerPrefabs[raptoroidID]);
        playerObject.transform.position = playerSpawnPoint.position;

        int gunID = GameManager.Instance.tutorialMode ? 0 : GameManager.Instance.EquippedWeapon;
        playerObject.GetComponentInChildren<ProjectileSpawner>().SetWeaponData(playerWeaponDataBank[gunID]);

        AudioSource playerWeaponAudioSource = playerObject.GetComponent<AudioSource>();
        playerWeaponAudioSource.clip = playerWeaponDataBank[gunID].shotSound;

        if (!PlayerPrefs.HasKey("sfxOn"))
        {
            PlayerPrefs.SetInt("sfxOn", 1);
        }

        if (!PlayerPrefs.HasKey("sfxVol"))
        {
            PlayerPrefs.SetFloat("sfxVol", 1);
        }

        playerWeaponAudioSource.volume = PlayerPrefs.GetInt("sfxOn") * PlayerPrefs.GetFloat("sfxVol");

        if(PlayerPrefs.GetInt("joystick") == 1){
            MakeJoyStick();
        }
        
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

        enemyHitParticles = new LinkedPool<GameObject>(MakeEnemyParticles, PoolCommons.OnGetFromPool, PoolCommons.OnReleaseToPool, PoolCommons.OnPoolItemDestroy, false, 1000);

        killCounter.text = string.Format("0 / {0}", enemyKillRequirement);

        oakNuts = new LinkedPool<GameObject>(MakeOakNut, PoolCommons.OnGetFromPool, PoolCommons.OnReleaseToPool, PoolCommons.OnPoolItemDestroy, false, 100);
        StartCoroutine(StartOakNutSpawn());

        mines = new LinkedPool<GameObject>(MakeMine, PoolCommons.OnGetFromPool, PoolCommons.OnReleaseToPool, PoolCommons.OnPoolItemDestroy, false, 100);
        StartCoroutine(StartMineSpawn());

        hills = new LinkedPool<GameObject>(MakeHill, PoolCommons.OnGetFromPool, PoolCommons.OnReleaseToPool, PoolCommons.OnPoolItemDestroy, false, 100);
        StartCoroutine(StartHillSpawn());
        UpdateScoreDisplay();
    }

    GameObject MakeJoyStick(){
        return joystick = Instantiate(joystickPrefab);
    }

    #region Obstacle Spawning
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
        oakNut.transform.position = new Vector3(UnityEngine.Random.Range(HorizontalLowerBound, HorizontalUpperBound), 10f, 0f); // Randomize spawn position
        oakNut.SetActive(true);
    }

    IEnumerator StartMineSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // Spawn mine every 10 seconds
            SpawnMine();
        }
    }

    void SpawnMine()
    {
        GameObject mine = mines.Get();
        mine.transform.position = new Vector3(0f, 10f, 0f); // Randomize spawn position
        mine.SetActive(true);
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
        hill.transform.position = new Vector3(Random.Range(HorizontalLowerBound, HorizontalUpperBound), 10f, 0f); // Randomize spawn position
        hill.SetActive(true);
    }
    #endregion

    void OnDisable()
    {
        StopAllCoroutines(); // Stop spawning coroutine when the stage ends
    }

    #region Pool Functions
    // Pooling functions
    GameObject MakeOakNut()
    {
        GameObject oakNut = Instantiate(oakNutPrefab);
        oakNut.SetActive(false); // Set inactive initially
        return oakNut;
    }

    GameObject MakeMine()
    {
        GameObject mine = Instantiate(minePrefab);
        mine.SetActive(false); // Set inactive initially
        return mine;
    }

    GameObject MakeHill()
    {
        GameObject hill = Instantiate(hillPrefab);
        hill.SetActive(false); // Set inactive initially
        return hill;
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

    // End of Pooling functions
    #endregion

    #region Gem Handling
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
    #endregion

    #region Boss
    public void UpdateBossHealthBar(float healthRatio)
    {
        bossHealthBarRect.sizeDelta = new Vector2(bossHealthBarWidth * healthRatio, bossHealthBarHeight);
    }

    public void SetBossHintTable(string tableName)
    {
        hint.SetTable(tableName);
    }

    public void DisplayBossHint(string msgKey, float duration = 3f)
    {
        StartCoroutine(BossHintDisplaySequence(msgKey, duration));
    }

    IEnumerator BossHintDisplaySequence(string msgKey, float duration)
    {
        bossHintCanvas.SetActive(true);

        hint.SetEntry(msgKey);
        hint.RefreshString();

        Time.timeScale = 1 / 64f;
        Coroutine warningFlash = StartCoroutine(WarningSignFlash());
        yield return new WaitForSecondsRealtime(duration);
        StopCoroutine(warningFlash);
        Time.timeScale = 1;
        bossHintCanvas.SetActive(false);
    }

    IEnumerator WarningSignFlash()
    {
        bool signOn = true;
        while (true)
        {
            warningSign.enabled = signOn;
            yield return new WaitForSecondsRealtime(0.3f);
            signOn = !signOn;
        }
    }
    #endregion

    public void ToggleOaknutScreen(bool toggle, float duration = 0)
    {
        OaknutHitScreen.SetActive(toggle);
        
        if (toggle)
        {
            ControlRestoreProgressBar progressBarControl = OaknutHitScreen.GetComponentInChildren<ControlRestoreProgressBar>();
            progressBarControl.ResetDuration(duration);
        }
    }

    #region Enemy Management
    public void OnEnemyDefeated()
    {
        killCounter.text = string.Format("{0} / {1}", ++enemyKillCount, enemyKillRequirement);

        if (enemyKillCount >= enemyKillRequirement)
        {
            EndStage(true);
        }
    }

    public Transform GetClosestEnemyTransform(Vector2 point)
    {
        Transform ret = null;
        float minDistanceSquare = float.MaxValue;

        foreach (Transform et in enemyTransforms)
        {
            if (et == null)
            {
                continue;
            }

            // Use sqrMagnitude instead of actual distance since we won't need to know how far away the closest
            // target actually is.
            float currentDistanceSquare = ((Vector2)et.position - point).sqrMagnitude;
            if (currentDistanceSquare < minDistanceSquare)
            {
                minDistanceSquare = currentDistanceSquare;
                ret = et;
            }
        }

        return ret;
    }

    public void RegisterEnemyTransform(Transform t)
    {
        enemyTransforms.Add(t);
    }

    public void UnregisterEnemyTransform(Transform t)
    {
        enemyTransforms.Remove(t);
    }
    #endregion

    public void OnPlayerDefeated()
    {
        EndStage(false);
    }

    #region Level End
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
        GameManager.Instance.updateProgress1("Quest 2", enemyKillCount);

        if (playerWin)
        {
            StartCoroutine(LevelEndGemCollectChance());
        }
        else
        {
            if (joystick != null)
            {
                joystick.SetActive(false);
            }

            ToggleOaknutScreen(false);

            bgmSelector.PlayEndMusic("lose");
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

            if (GameManager.Instance.tutorialMode)
            {
                tutorialEndScreen.SetActive(true);
                tutorialEndTextLocalizeEvent.SetEntry("TutorialLose");
                tutorialEndTextLocalizeEvent.RefreshString();
            }
        }
    }

    IEnumerator LevelEndGemCollectChance()
    {
        while (gemsWaiting > 0)
        {
            yield return new WaitForEndOfFrame();
        }

        ToggleOaknutScreen(false);
        Time.timeScale = 0;

        if (isBossStage)
        {
            bgmSelector.PlayEndMusic("bossWin");
            bossWinScreen.SetActive(true);
            runEndScoreCanvas.SetActive(true);
            runEndScoreText.text = GameManager.Instance.GetCurrentScore().ToString();
            newHiScoreNotice.SetActive(GameManager.Instance.HighScoreChanged);

            if (GameManager.Instance.tutorialMode)
            {
                tutorialEndScreen.SetActive(true);
                tutorialEndTextLocalizeEvent.SetEntry("TutorialWin");
                tutorialEndTextLocalizeEvent.RefreshString();
            }
        }
        else
        {
            if (joystick != null)
            {
                joystick.SetActive(false);
            }

            bgmSelector.PlayEndMusic("win");
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
            bgmSelector.PlayEndMusic("lose");
            if (isBossStage)
            {
                // BossID is 0-indexed for array access purposes. Add 1 to match the ID number
                GameAnalytics.SetCustomDimension01(((Bosses)GameManager.Instance.BossID).ToString());
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "Mission", "Boss");
                GameAnalytics.SetCustomDimension01(null);
            }
            else
            {
                GameAnalytics.SetCustomDimension02(GameManager.Instance.MapTier.ToString());
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "Mission", "ActionStage");
                GameAnalytics.SetCustomDimension02(null);
            }
        }

        if (GameManager.Instance.tutorialMode)
        {
            GameManager.Instance.tutorialMode = false;
            GameAnalytics.NewDesignEvent("Tutorial:Complete");
            PlayerPrefs.SetInt("TutorialComplete", 1);
        }

        GameManager.Instance.ClearMapInfo();
        GameManager.Instance.ResetScore();

        float multiplier = playerWon ? 1f : 0.8f;
        GameManager.Instance.CommitCollectedGems(multiplier);

        GameManager.Instance.SaveGame();
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
    #endregion
}
