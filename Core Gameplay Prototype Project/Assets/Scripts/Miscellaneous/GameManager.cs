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
    int totalGems = 0;

    Map[] generatedMaps = null;
    int mapIndex = 0;
    int currentMapTier = 0;
    int totalShips = 2;
    int totalGuns = 2;
    List<MapNode> visitedNodes = new List<MapNode>();
    List<byte> availableShips;
    List<byte> availableGuns;

    EnemyFormation[] selectedStageFormations;

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

            availableShips = new List<byte>(totalShips / 8 + 1);
            availableGuns = new List<byte>(totalGuns / 8 + 1);

            if (!PlayerPrefs.HasKey("Locale"))
            {
                PlayerPrefs.SetInt("Locale", 0);
            }
            SetLocale(PlayerPrefs.GetInt("Locale"));

            EnemySpawner.LoadEnemyFormations();
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLocale(int id)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(gameLocales[id]);
        PlayerPrefs.SetInt("Locale", id);
    }

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
}
