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

    [SerializeField] LocaleIdentifier[] gameLocales;

    int pendingGems = 0;
    int totalGems = 500;

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

            if (!PlayerPrefs.HasKey("LocaleIntID"))
            {
                PlayerPrefs.SetInt("LocaleIntID", 0);
            }
            SetLocale(PlayerPrefs.GetInt("LocaleIntID"));

            EnemySpawner.LoadEnemyFormations();
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
    public void CollectGems(int gemAmount){
        pendingGems += gemAmount;
    }

    public int GetCurrentGems(){
        return pendingGems;
    }

    public void CommitCollectedGems(float modifier)
    {
        totalGems += Mathf.CeilToInt(modifier * pendingGems);
        pendingGems = 0;
    }

    public int GetTotalGems()
    {
        return totalGems;
    }

    public int GetCurrentScore()
    {
        return score;
    }
    #endregion

    #region ITEM_PURCHASE
    public void PurchaseItem(ShopItemData purchasedItem)
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
        if (type == ItemType.Weapon)
        {
            availableGuns[itemNumber / 8] |= (byte)(1 << itemNumber % 8);
        }
        else
        {
            availableShips[itemNumber / 8] |= (byte)(1 << itemNumber % 8);
        }
    }

    public bool ItemUnlocked(ItemType type, int itemNumber)
    {
        if (type == ItemType.Weapon)
        {
            return (availableGuns[itemNumber / 8] & (1 << itemNumber % 8)) == 1;
        }
        else
        {
            return (availableShips[itemNumber / 8] & (1 << itemNumber % 8)) == 1;
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
