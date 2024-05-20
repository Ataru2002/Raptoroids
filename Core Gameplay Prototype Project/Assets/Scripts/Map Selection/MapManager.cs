using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using TMPro;
using GameAnalyticsSDK;

public class MapManager : MonoBehaviour
{
    static MapManager instance;
    public static MapManager Instance { get { return instance; } }

    const int mapWidth = 5;
    const int mapHeight = 4;
    const int mapCount = 3;

    Sprite[] bossNodeSprites;

    Map[] maps = null;
    MapNode selectedNode = null;

    RectTransform nodeGroupTransform;

    ObjectPool<GameObject> drawnLinePool;
    List<GameObject> drawnLines = new List<GameObject>();

    [SerializeField] RectTransform screenCanvasTransform;

    [SerializeField] Sprite combatNodeSprite;
    [SerializeField] Sprite treasureNodeSprite;

    [SerializeField] GameObject mapScreen;
    [SerializeField] RectTransform[] mapSelectButtonTransforms;
    [SerializeField] GameObject mapLineContainer;
    [SerializeField] Image[] mapCellIcons;
    [SerializeField] RectTransform bossNodeTransform;
    [SerializeField] Image bossButtonImage;
    [SerializeField] Button bossButton;

    [SerializeField] GameObject actionStagePrompt;
    [SerializeField] TextMeshProUGUI enemyCountText;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            bossNodeSprites = Resources.LoadAll<Sprite>("BossNodeSprites");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        drawnLinePool = new ObjectPool<GameObject>(MakeLine, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolLine, true, mapWidth * mapHeight);

        maps = GameManager.Instance.GetMaps();
        nodeGroupTransform = GetComponentInChildren<GridLayoutGroup>().GetComponent<RectTransform>();
        SelectMap(GameManager.Instance.MapIndex);
    }

    // Line pool functions
    GameObject MakeLine()
    {
        GameObject line = new GameObject();
        line.name = "Map Line";

        RectTransform lineRect = line.AddComponent<RectTransform>();
        lineRect.SetParent(mapLineContainer.transform);
        lineRect.pivot = new Vector2(0.5f, 1);

        Image lineImg = line.AddComponent<Image>();
        lineImg.color = Color.black;

        return line;
    }

    void OnTakeFromPool(GameObject line)
    {
        line.SetActive(true);
    }

    void OnReturnedToPool(GameObject line)
    {
        line.SetActive(false);
    }

    void OnDestroyPoolLine(GameObject line)
    {
        Destroy(line);
    }
    // End of line pool functions

    public Map[] GenerateMaps()
    {
        Map[] maps = new Map[mapCount];

        int[] selectableBosses = Enumerable.Range(0, EnemySpawner.AvailableBossCount).ToArray();
        for (int i = selectableBosses.Length - 1; i > 0; i--)
        {
            int swapIndex = Random.Range(0, i);

            int temp = selectableBosses[swapIndex];
            selectableBosses[swapIndex] = selectableBosses[i];
            selectableBosses[i] = temp;
        }

        for (int m = 0; m < maps.Length; m++)
        {
            Map currentMap = new Map(mapWidth, mapHeight);
            currentMap.Populate(Random.Range(mapWidth - 1, mapWidth + 1));
            print(selectableBosses[m]);
            currentMap.SetBossID(selectableBosses[m]);
            maps[m] = currentMap;
        }

        // Maps regenerating signifies the start of a new mission
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "Mission");

        return maps;
    }

    void SelectMap(int mapIndex)
    {
        GameManager.Instance.MapIndex = mapIndex;

        for (int b = 0; b < mapSelectButtonTransforms.Length; b++)
        {
            float buttonHeight = b == mapIndex ? 100 : 75;
            mapSelectButtonTransforms[b].sizeDelta = new Vector2(135, buttonHeight);
        }

        DrawMap(mapIndex);
    }

    public void ReselectMap(int mapIndex)
    {
        SelectMap(mapIndex);
        ButtonSFXPlayer.Instance.PlaySFX("MapSelect");
    }

    MapNode GetNodeInfo(int nodeIndex)
    {
        int y = nodeIndex / mapWidth;
        int x = nodeIndex % mapWidth;
        return maps[GameManager.Instance.MapIndex].GetNodes()[y, x];
    }

    public void SelectNode(int nodeIndex)
    {
        selectedNode = GetNodeInfo(nodeIndex);
        MapNodeType selectedNodeType = selectedNode.Type;

        if (selectedNodeType == MapNodeType.Treasure)
        {
            GameManager.Instance.MarkNodeVisited(selectedNode);
            GoToTreasureRoom();
        }
        else
        {
            ToggleActionPrompt(nodeIndex);
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        ButtonSFXPlayer.Instance.PlaySFX("ToMenus");
    }

    public void ToggleActionPrompt(int nodeIndex)
    {
        actionStagePrompt.SetActive(nodeIndex >= 0);

        if (nodeIndex < 0)
        {
            return;
        }

        selectedNode = GetNodeInfo(nodeIndex);
        int enemyCount = 0;
        foreach (EnemyFormation formation in selectedNode.GetEnemyFormations())
        {
            enemyCount += formation.GetEnemyCount();
        }

        GameManager.Instance.SetStageFormations(selectedNode.GetEnemyFormations());

        enemyCountText.text = enemyCount.ToString();
    }

    public void GoToBoss()
    {
        GameManager.Instance.BossID = maps[GameManager.Instance.MapIndex].GetBossID();
        GoToAction();
    }

    public void GoToAction()
    {
        GameManager.Instance.MarkNodeVisited(selectedNode);
        SceneManager.LoadScene("CombatStagePrototype", LoadSceneMode.Single);
        ButtonSFXPlayer.Instance.PlaySFX("ToAction");
    }

    public void GoToTreasureRoom()
    {
        SceneManager.LoadScene("TreasureStagePrototype", LoadSceneMode.Single);
        ButtonSFXPlayer.Instance.PlaySFX("TreasureGet");
    }

    void UpdateMapButtons()
    {
        for (int i = 0; i < mapCount; i++)
        {
            if (GameManager.Instance.MapTier > 0)
            {
                Button mapButton = mapSelectButtonTransforms[i].GetComponent<Button>();
                mapButton.interactable = GameManager.Instance.MapIndex == i;
            }
        }
    }

    void DrawMap(int mapIndex)
    {
        ClearLines();
        DrawMap(maps[mapIndex]);
    }

    void DrawMap(Map map)
    {
        MapNode[,] mapNodes = map.GetNodes();

        // Start working from the end so that the lines linking from the
        // start to the end can be drawn without making a second pass
        // through the entire grid.
        for (int y = mapHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                Image icon = GetIconAtCoords(x, y);
                MapNode node = mapNodes[y, x];

                if (node.Type == MapNodeType.None)
                {
                    icon.enabled = false;
                    icon.GetComponent<Button>().enabled = false;
                }
                else
                {
                    icon.enabled = true;
                    icon.sprite = node.Type == MapNodeType.Combat ? combatNodeSprite : treasureNodeSprite;
                    icon.rectTransform.localPosition = node.PositionOffset;

                    Button iconButton = icon.GetComponent<Button>();
                    iconButton.enabled = true;

                    int currentTier = GameManager.Instance.MapTier;
                    MapNode lastVisitedNode = GameManager.Instance.LastVisitedNode;
                    bool selectable = currentTier == y && (currentTier == 0 || lastVisitedNode.GetConnections().Contains(node));
                    iconButton.interactable = selectable;

                    icon.GetComponent<MapNodeAnimator>().ToggleAnimation(selectable);
                }
            }

            // Force a layout rebuild immediately so that the node positions
            // are up to date and ready to be used to draw lines.
            LayoutRebuilder.ForceRebuildLayoutImmediate(nodeGroupTransform);

            // Take a second pass through the row to draw the lines.
            for (int n = 0; n < mapWidth; n++)
            {
                Image icon = GetIconAtCoords(n, y);
                MapNode node = mapNodes[y, n];

                node.absolutePosition = icon.rectTransform.TransformPoint(Vector3.zero);

                if (node.Type != MapNodeType.None)
                {
                    if (node.IsFinalNode())
                    {
                        DrawLine(node.absolutePosition, bossNodeTransform.position);
                    }
                    else
                    {
                        foreach (MapNode next in node.GetConnections())
                        {
                            DrawLine(node.absolutePosition, next.absolutePosition);
                        }
                    }
                }
            }
        }

        UpdateMapButtons();

        bossButtonImage.sprite = bossNodeSprites[map.GetBossID()];
        bossButton.interactable = GameManager.Instance.MapTier >= mapHeight;
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        GameObject line = drawnLinePool.Get();
        
        RectTransform lineRect = line.GetComponent<RectTransform>();
        lineRect.position = start;
        lineRect.sizeDelta = new Vector2(5 * screenCanvasTransform.lossyScale.x, (end - start).magnitude);
        
        float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;

        lineRect.rotation = Quaternion.Euler(0, 0, angle + 90);

        drawnLines.Add(line);
    }

    void ClearLines()
    {
        foreach (GameObject line in drawnLines)
        {
            drawnLinePool.Release(line);
        }
        drawnLines.Clear();
    }

    Image GetIconAtCoords(int x, int y)
    {
        if (y < 0 || x < 0 || y >= mapHeight || x >= mapWidth)
        {
            throw new System.IndexOutOfRangeException();
        }

        return mapCellIcons[mapWidth * y + x];
    }
}

public enum MapNodeType
{
    None,
    Combat,
    Treasure
}

public class Map
{
    MapNode[,] mapNodes = null;
    int bossID = 0;

    int width;
    int height;

    // Construct an empty map of the specified size
    public Map(int mapWidth, int mapHeight)
    {
        mapNodes = new MapNode[mapHeight, mapWidth];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                mapNodes[y, x] = new MapNode();
            }
        }

        width = mapWidth;
        height = mapHeight;
    }

    public void Populate(int seeds)
    {
        // Range code acquired from https://stackoverflow.com/questions/10681882/create-c-sharp-int-with-value-as-0-1-2-3-length
        int[] slotNumbers = Enumerable.Range(0, width).ToArray();

        // Shuffle slot numbers
        for (int s = slotNumbers.Length - 1; s > 0; s--)
        {
            int t = Random.Range(0, s + 1);

            int temp = slotNumbers[s];
            slotNumbers[s] = slotNumbers[t];
            slotNumbers[t] = temp;
        }

        for (int i = 0; i < seeds; i++)
        {
            int x = slotNumbers[i % width];
            int y = 0;
            MapNode previous = null;

            while (y < height)
            {
                MapNode current = mapNodes[y, x];

                if (previous != null)
                {
                    previous.AddLink(current);
                }

                if (current.IsClear())
                {
                    mapNodes[y, x].Randomize(y);
                }

                previous = current;
                
                x = Random.Range(Mathf.Max(0, x - 1), Mathf.Min(width, (x + 1) + 1));
                y++;
            }
        }
    }

    public void SetBossID(int val)
    {
        bossID = val;
    }

    public int GetBossID()
    {
        return bossID;
    }

    public MapNode[,] GetNodes()
    {
        return mapNodes;
    }
}

public class MapNode
{
    MapNodeType type;
    EnemyFormation[] nodeEnemies = null;
    Vector2 positionOffset;
    List<MapNode> nextNodes;

    public Vector2 absolutePosition;

    public MapNode()
    {
        type = MapNodeType.None;
        positionOffset = Vector2.zero;
        nextNodes = new List<MapNode>();
    }

    public void Randomize(int nodeTier)
    {
        if (nodeTier == 0)
        {
            type = MapNodeType.Combat;
        }
        else
        {
            type = Random.Range(0.0f, 1.0f) <= 0.75f ? MapNodeType.Combat : MapNodeType.Treasure;
        }

        if (type == MapNodeType.Combat)
        {
            nodeEnemies = EnemySpawner.PrepareFormations(nodeTier);
        }

        float xOffset = Random.Range(-10.0f, 10.0f);
        float yOffset = Random.Range(-10.0f, 10.0f);
        positionOffset = new Vector2(xOffset, yOffset);
    }

    public EnemyFormation[] GetEnemyFormations()
    {
        return nodeEnemies;
    }

    public MapNodeType Type { get { return type; } }

    public Vector2 PositionOffset { get {  return positionOffset; } }

    public void AddLink(MapNode next)
    {
        nextNodes.Add(next);
    }

    public List<MapNode> GetConnections()
    {
        return nextNodes;
    }

    public bool IsFinalNode()
    {
        return nextNodes.Count == 0;
    }

    public bool IsClear()
    {
        return type == MapNodeType.None;
    }
}
