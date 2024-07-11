using GameAnalyticsSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    public QuestGetter quests;

    [SerializeField] LoadTimeMeasure loadTimeMeasure;

    public bool tutorialMode = false;
    public bool tutorialGemMessageDisplayed = false;
    public bool tutorialTreasureRoomMessageDisplayed = false;

    SaveData playerData = null;
    const string saveFileName = "raptoroidssave.dat";
    string saveFilePath;

    [SerializeField] LocaleIdentifier[] gameLocales;

    int totalShips;
    int totalGuns;

    #region PERSISTENT_DATA
    // TODO: if there is time, replace the following getters with references to the corresponding variable in playerData
    int PendingGems { get { return playerData.pendingGems; } set { playerData.pendingGems = value; } }
    int TotalGems { get { return playerData.totalGems; } set { playerData.totalGems = value; } }
    public int EnemiesSinceLastTreasureRoom { get { return playerData.enemiesKilledSinceLastTreasureRoom; } set { playerData.enemiesKilledSinceLastTreasureRoom = value; } }

    Map[] GeneratedMaps { get { return playerData.generatedMaps; } set { playerData.generatedMaps = value; } }
    
    public int MapIndex
    {
        get { return playerData.mapIndex; }
        set { playerData.mapIndex = value; }
    }

    public int MapTier { get { return playerData.currentMapTier; } private set { playerData.currentMapTier = value; } }

    public List<MapNode> VisitedNodes { get { return playerData.visitedNodes; } }

    List<byte> AvailableRaptoroids { get { return playerData.ownedRaptoroids; } }

    List<byte> AvailableGuns { get { return playerData.ownedWeapons; } }

    public int EquippedRaptoroid { get { return playerData.equippedRaptoroid; } private set { playerData.equippedRaptoroid = value; } }
    public int EquippedWeapon { get { return playerData.equippedWeapon; } private set { playerData.equippedWeapon = value; } }

    int Score { get { return playerData.score; } set { playerData.score = value; } }
    int HiScore { get { return playerData.hiScore; } set { playerData.hiScore = value; } }
    #endregion

    MapNode selectedNode = null;
    EnemyFormation[] selectedStageFormations;

    public bool HighScoreChanged { get; private set; } = false;

    public int BossID { get; set; } = 0;

    // For analytics data regarding gems
    MissionGemsContactPoint gemsContactPoint;

    private float updateInterval = 5.0f; // interval in second
    private float timer = 0.0f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;

            CombatSFXManager.LoadCombatSFX();
            ProjectilePoolManager.LoadProjectiles();
            EnemySpawner.LoadEnemyFormations();
            ShopManager.LoadShopItems();
            LoadoutManager.LoadItems();

            totalShips = LoadoutManager.GetItemCount(ItemType.Raptoroid);
            totalGuns = LoadoutManager.GetItemCount(ItemType.Weapon);

            print($"Raptoroid count: {totalShips}, weapon count: {totalGuns}");

            saveFilePath = Application.persistentDataPath + $"/{saveFileName}";
            if (File.Exists(saveFilePath))
            {
                string saveFileString = File.ReadAllText(saveFilePath);
                playerData = JsonUtility.FromJson<SaveData>(saveFileString);
            }
            else
            {
                playerData = new SaveData();
            }

            int shipVectorBytes = totalShips / 8 + 1;
            while (AvailableRaptoroids.Count < shipVectorBytes)
            {
                AvailableRaptoroids.Add(0);
            }
            UnlockItem(ItemType.Raptoroid, 0);
            
            int gunVectorBytes = totalGuns / 8 + 1;
            while (AvailableGuns.Count < gunVectorBytes)
            {
                AvailableGuns.Add(0);
            }
            UnlockItem(ItemType.Weapon, 0);

            if (!PlayerPrefs.HasKey("LocaleIntID"))
            {
                PlayerPrefs.SetInt("LocaleIntID", 0);
            }
            SetLocale(PlayerPrefs.GetInt("LocaleIntID"));

            gemsContactPoint = GetComponent<MissionGemsContactPoint>();
        }
    }

    private void Start()
    {
        LoadingDataStart("Quest 2");
        LoadingDate();
        GameAnalytics.Initialize();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            resetQuests("Quest 2");
            timer = 0.0f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ScreenCapture.CaptureScreenshot($"ScreenCap{DateTime.Now.Ticks}.png");
        }
    }

    public void LoadingDataStart(string ID)
    {
        quests = GetComponent<QuestGetter>();
        quests.LoadData(ID);
    }

    public void LoadingDate()
    {
        quests = GetComponent<QuestGetter>();
        quests.LoadDate();
    }

    public void updateProgress1(string ID, int amount) 
    {
        quests.LoadData(ID);
        quests.dscurrent.progress += amount;
        quests.SaveData(ID);
    }

    public void updateProgress2(string ID, int amount)
    {
        quests.LoadData(ID);
        quests.dscurrent.progress = amount;
        quests.SaveData(ID);
    }

    public void resetQuests(string ID)
    {
        LoadingDate();
        DateTime lastTimeStamp;
        DateTime currentTimeStamp;
        lastTimeStamp = DateTime.ParseExact(quests.tscurrent.lastSave, "yyyy-MM-dd HH:mm:ss", null);
        currentTimeStamp = DateTime.Now;

        Debug.Log(quests.tscurrent.lastSave);

        TimeSpan diff = currentTimeStamp - lastTimeStamp;
        if (diff.Days >= 7)
        {
            updateProgress2(ID, 0);
            quests.SaveDate(currentTimeStamp);
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            GameAnalytics.StartSession();
        }
        else
        {
            GameAnalytics.EndSession();
        }
    }

    #region LOCALIZATION
    public void SetLocale(int id)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(gameLocales[id]);
        PlayerPrefs.SetString("LocaleStringID", gameLocales[id].ToString());
        PlayerPrefs.SetInt("LocaleIntID", id);
    }

    public Locale GetLocale(int id)
    {
        return LocalizationSettings.AvailableLocales.GetLocale(gameLocales[id]);
    }
    #endregion

    #region MAP_PROGRESSION
    // Map info
    public Map[] GetMaps()
    {
        // Return existing maps. If none exists, generate new maps.
        if (GeneratedMaps == null || GeneratedMaps.Length == 0)
        {
            // Assume the maps will be generated for the first time in the maps scene.
            GeneratedMaps = MapManager.Instance.GenerateMaps();
        }

        else if (!tutorialMode && GeneratedMaps.Length < 3)
        {
            // Clear out leftover data from tutorial
            ClearMapInfo();

            GeneratedMaps = MapManager.Instance.GenerateMaps();
        }

        // Check for missing map data. If map data went missing, generate new maps
        else
        {
            bool mapMissing = false;
            foreach (Map map in GeneratedMaps)
            {
                if (map == null)
                {
                    mapMissing = true;
                    break;
                }
            }

            if (mapMissing)
            {
                ClearMapInfo();
                GeneratedMaps = MapManager.Instance.GenerateMaps();
            }
        }

        // Save the generated maps
        SaveGame();
        return GeneratedMaps;
    }

    public void ClearStageFormations()
    {
        selectedStageFormations = null;
    }

    public void SetStageFormations(EnemyFormation[] formations)
    {
        selectedStageFormations = formations;
    }

    public EnemyFormation[] GetStageFormations()
    {
        return selectedStageFormations;
    }

    public void AdvanceMapProgress()
    {
        MarkSelectedNodeVisited();
        MapTier++;
        SaveGame();
    }

    // The following function is included for the purpose of cheats
    public void SetMapTier(int val)
    {
        MapTier = val;
    }

    public void SelectNode(MapNode node)
    {
        selectedNode = node;
    }

    public void MarkSelectedNodeVisited()
    {
        VisitedNodes.Add(selectedNode);
    }

    public MapNode LastVisitedNode
    {
        get { return VisitedNodes.Count > 0 ? VisitedNodes[VisitedNodes.Count - 1] : null; }
    }

    public void ClearMapInfo()
    {
        GeneratedMaps = null;
        MapIndex = 0;
        MapTier = 0;
        VisitedNodes.Clear();
    }
    #endregion

    #region GEMS
    public void CollectGems(int gemAmount)
    {
        PendingGems += gemAmount;
    }

    public int GetCurrentGems()
    {
        return PendingGems;
    }

    public void CommitCollectedGems(float modifier)
    {
        TotalGems += Mathf.CeilToInt(modifier * PendingGems);
        PendingGems = 0;
        SendGemAnalyticsData();
    }

    public int GetTotalGems()
    {
        return TotalGems;
    }

    public int GetCurrentScore()
    {
        return Score;
    }

    public void UpdateGemSourceData(GemSources gemSource, int delta)
    {
        gemsContactPoint.SetSourceData(gemSource, delta);
    }

    public void SetGemPenaltyData(int val)
    {
        gemsContactPoint.SetPenaltyValue(val);
    }

    void SendGemAnalyticsData()
    {
        gemsContactPoint.SendData();
        gemsContactPoint.ResetData();
    }
    #endregion

    #region ITEM_PURCHASE
    public void PurchaseItem(ItemData purchasedItem)
    {
        int bitVectorKey = purchasedItem.itemNumber / 8;
        int bitIndex = purchasedItem.itemNumber % 8;

        List<byte> targetList = purchasedItem.itemType == ItemType.Weapon ? AvailableGuns : AvailableRaptoroids;
        
        byte bitMask = (byte)(1 << bitIndex);
        targetList[bitVectorKey] |= bitMask;

        TotalGems -= purchasedItem.gemCost;

        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Gem", purchasedItem.gemCost, purchasedItem.itemType.ToString(), purchasedItem.itemNumber.ToString());
        SaveGame();
    }
    #endregion

    #region LOADOUT
    public void UnlockItem(ItemType type, int itemNumber)
    {
        int arrayIndex = itemNumber / 8;
        int bitIndex = itemNumber % 8;

        if (type == ItemType.Weapon)
        {
            AvailableGuns[arrayIndex] |= (byte)(1 << bitIndex);
        }
        else
        {
            AvailableRaptoroids[arrayIndex] |= (byte)(1 << bitIndex);
        }
    }

    public bool ItemUnlocked(ItemType type, int itemNumber)
    {
        int arrayIndex = itemNumber / 8;
        int bitIndex = itemNumber % 8;

        if (type == ItemType.Weapon)
        {
            return (AvailableGuns[arrayIndex] & (1 << bitIndex)) != 0;
        }
        else
        {
            return (AvailableRaptoroids[arrayIndex] & (1 << bitIndex)) != 0;
        }
    }

    public void EquipItem(ItemType type, int itemNumber)
    {
        if (type == ItemType.Weapon)
        {
            EquippedWeapon = itemNumber;
        }
        else if (type == ItemType.Raptoroid)
        {
            EquippedRaptoroid = itemNumber;
        }
    }
    #endregion

    #region SCORE
    public void ResetScore()
    {
        Score = 0;
        HighScoreChanged = false;
    }

    public void AddScore(int val)
    {
        Score += val;
        
        if (Score > HiScore)
        {
            HighScoreChanged = true;
            HiScore = Score;
        }
    }

    public int GetHighScore()
    {
        return HiScore;
    }
    #endregion

    #region UTILITIES

    public void SaveGame()
    {
        // Pretty print on for debugging. Turn off to minimize save file size
        string saveString = JsonUtility.ToJson(playerData, false);
        File.WriteAllText(saveFilePath, saveString);
    }

    public void StartTutorial()
    {
        ClearMapInfo();
        tutorialMode = true;
    }
    #endregion
}

[Serializable]
public class SaveData
{
    public int pendingGems = 0;
    public int totalGems = 0;
    public int score = 0;
    public int hiScore = 0;
    public int enemiesKilledSinceLastTreasureRoom;

    public int equippedRaptoroid = 0;
    public int equippedWeapon = 0;

    public List<byte> ownedRaptoroids = new List<byte>();
    public List<byte> ownedWeapons = new List<byte>();

    public Map[] generatedMaps = null;
    public int mapIndex;
    public int currentMapTier = 0;
    public List<MapNode> visitedNodes = new List<MapNode>();
}
