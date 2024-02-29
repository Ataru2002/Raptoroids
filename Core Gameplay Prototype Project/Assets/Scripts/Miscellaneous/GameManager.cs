using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    int currentGems = 0;
    Map[] generatedMaps = null;
    int mapIndex = 0;
    int currentMapTier = 0;
    List<MapNode> visitedNodes = new List<MapNode>();

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

    public void AdvanceMapProgress()
    {
        currentMapTier++;
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

    public void collectGems(int gemAmount){
        currentGems += gemAmount;
    }

    public int getCurrentGems(){
        return currentGems;
    }
}
