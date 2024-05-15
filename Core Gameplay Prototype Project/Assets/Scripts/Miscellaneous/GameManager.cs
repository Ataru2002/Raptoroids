using GameAnalyticsSDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    public QuestGetter quests;

    [SerializeField] LocaleIdentifier[] gameLocales;

    int pendingGems = 0;
    int totalGems = 0;

    Map[] generatedMaps = null;
    int mapIndex = 0;
    int currentMapTier = 0;
    List<MapNode> visitedNodes = new List<MapNode>();

    const int totalShips = 2;
    const int totalGuns = 2;
    List<byte> availableShips;
    List<byte> availableGuns;

    EnemyFormation[] selectedStageFormations;

    int score;
    int hiScore;
    public bool HighScoreChanged { get; private set; } = false;

    public int BossID { get; set; } = 0;

    MissionGemsContactPoint gemsContactPoint;

  
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

            // TODO: load from save file instead
            int shipVectorBytes = totalShips / 8 + 1;
            availableShips = new List<byte>();
            for (int s = 0; s < shipVectorBytes; s++)
            {
                availableShips.Add(0);
            }
            UnlockItem(ItemType.Raptoroid, 0);
            
            int gunVectorBytes = totalGuns / 8 + 1;
            availableGuns = new List<byte>();
            for (int g = 0; g < gunVectorBytes; g++)
            {
                availableGuns.Add(0);
            }
            UnlockItem(ItemType.Weapon, 0);

            // Remove these after playtesting - ensures that the loadout is reset to default
            // between sessions
            PlayerPrefs.SetInt("EquippedRaptoroid", 0);
            PlayerPrefs.SetInt("EquippedWeapon", 0);

            if (!PlayerPrefs.HasKey("LocaleIntID"))
            {
                PlayerPrefs.SetInt("LocaleIntID", 0);
            }
            SetLocale(PlayerPrefs.GetInt("LocaleIntID"));

            gemsContactPoint = GetComponent<MissionGemsContactPoint>();

            EnemySpawner.LoadEnemyFormations();
        }
    }
    private void Start()
    {
        LoadingDataStart();
        GameAnalytics.Initialize();
    }

    public void LoadingDataStart()
    {
        quests = GetComponent<QuestGetter>();
        quests.LoadData("Quest 2");
    }

    public void updateProgress(string ID, int amount) 
    {
        quests.LoadData(ID);
        quests.dscurrent.progress += amount;
        quests.SaveData(ID);
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
        if (generatedMaps == null)
        {
            // Assume the maps will be generated for the first time in the maps scene.
            generatedMaps = MapManager.Instance.GenerateMaps();
        }
        return generatedMaps;
    }

    public int MapIndex { 
        get { return mapIndex; }
        set { mapIndex = value; }
    }

    public int MapTier { get { return currentMapTier; } }

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
        currentMapTier++;
    }

    // The following function is included for the purpose of cheats
    public void SetMapTier(int val)
    {
        currentMapTier = val;
    }

    public void MarkNodeVisited(MapNode node)
    {
        visitedNodes.Add(node);
    }

    public MapNode LastVisitedNode
    {
        get { return visitedNodes.Count > 0 ? visitedNodes[visitedNodes.Count - 1] : null; }
    }

    public void ClearMapInfo()
    {
        generatedMaps = null;
        mapIndex = 0;
        currentMapTier = 0;
        visitedNodes.Clear();
    }
    #endregion

    #region GEMS
    public void CollectGems(int gemAmount)
    {
        pendingGems += gemAmount;
    }

    public int GetCurrentGems()
    {
        return pendingGems;
    }

    public void CommitCollectedGems(float modifier)
    {
        totalGems += Mathf.CeilToInt(modifier * pendingGems);
        pendingGems = 0;
        SendGemAnalyticsData();
    }

    public int GetTotalGems()
    {
        return totalGems;
    }

    public int GetCurrentScore()
    {
        return score;
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
    }
    #endregion

    #region ITEM_PURCHASE
    public void PurchaseItem(ItemData purchasedItem)
    {
        int bitVectorKey = purchasedItem.itemNumber / 8;
        int bitIndex = purchasedItem.itemNumber % 8;

        List<byte> targetList = purchasedItem.itemType == ItemType.Weapon ? availableGuns : availableShips;
        
        byte bitMask = (byte)(1 << bitIndex);
        targetList[bitVectorKey] |= bitMask;

        totalGems -= purchasedItem.gemCost;
    }
    #endregion

    #region LOADOUT
    public void UnlockItem(ItemType type, int itemNumber)
    {
        int arrayIndex = itemNumber / 8;
        int bitIndex = itemNumber % 8;

        if (type == ItemType.Weapon)
        {
            availableGuns[arrayIndex] |= (byte)(1 << bitIndex);
        }
        else
        {
            availableShips[arrayIndex] |= (byte)(1 << bitIndex);
        }
    }

    public bool ItemUnlocked(ItemType type, int itemNumber)
    {
        int arrayIndex = itemNumber / 8;
        int bitIndex = itemNumber % 8;

        if (type == ItemType.Weapon)
        {
            return (availableGuns[arrayIndex] & (1 << bitIndex)) != 0;
        }
        else
        {
            return (availableShips[arrayIndex] & (1 << bitIndex)) != 0;
        }
    }

    public void EquipItem(ItemType type, int itemNumber)
    {
        // TODO: use some other form of serialization
        // to make it harder for the player to manipulate the data illegitimately
        if (type == ItemType.Weapon)
        {
            PlayerPrefs.SetInt("EquippedWeapon", itemNumber);
        }
        else if (type == ItemType.Raptoroid)
        {
            PlayerPrefs.SetInt("EquippedRaptoroid", itemNumber);
        }
    }
    #endregion

    #region SCORE
    public void ResetScore()
    {
        score = 0;
        HighScoreChanged = false;
    }

    public void AddScore(int val)
    {
        score += val;
        
        if (score > hiScore)
        {
            HighScoreChanged = true;
            hiScore = score;
        }
    }

    public int GetHighScore()
    {
        return hiScore;
    }
    #endregion
}
