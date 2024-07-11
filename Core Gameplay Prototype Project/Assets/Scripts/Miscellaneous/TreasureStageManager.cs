using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class TreasureStageManager : MonoBehaviour
{
    int gemsCollectedInStage = 0;
    static TreasureStageManager instance;
    public static TreasureStageManager Instance { get { return instance; } }

    LinkedPool<GameObject> gemProjectiles;
    GameObject gemPrefab;
    [SerializeField] Transform playerSpawnPoint;
    [SerializeField] GameObject winScreen;
    [SerializeField] RectTransform winScreenSummaryBox;
    [SerializeField] TextMeshProUGUI timer;
    [SerializeField] float timeLimit = 10;
    [SerializeField] GameObject joystickPrefab;
    GameObject joystick; 
    float currentTime = 0;

    GameObject rewardSummaryPrefab;

    GameObject[] playerPrefabs;

    bool stageEnded = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            
            rewardSummaryPrefab = Resources.Load<GameObject>("Prefabs/UI Elements/StageSummaryItem");
            gemPrefab = Resources.Load<GameObject>("Prefabs/Combat Objects/Pickups/TreasureRoomGem");
            playerPrefabs = Resources.LoadAll<GameObject>("Prefabs/Raptoroids");
        }
    }

    void Start()
    {
        int prefabID = GameManager.Instance.tutorialMode ? 0 : GameManager.Instance.EquippedRaptoroid;
        GameObject player = Instantiate(playerPrefabs[prefabID]);
        player.transform.position = playerSpawnPoint.position;

        if (PlayerPrefs.GetInt("joystick") != 0)
        {
            MakeJoystick();
        }

        player.GetComponent<DoubleTapDetector>().enabled = false;
        player.GetComponent<RaptoroidAbility>().enabled = false;
        foreach (Transform child in player.transform)
        {
            child.gameObject.SetActive(false);
        }

        gemProjectiles = new LinkedPool<GameObject>(MakeGemProjectile, OnGetFromPool, OnReleaseToPool, OnPoolItemDestroy, false, 100);
        
    }

    void Update(){
        
        currentTime += Time.deltaTime;

        if(currentTime >= timeLimit){
            EndStage();
        }

        timer.text = Mathf.CeilToInt(timeLimit - currentTime).ToString();
    }


    GameObject MakeGemProjectile(){
        return Instantiate(gemPrefab);
    }

    public GameObject GetDiamondProjectile()
    {
        return gemProjectiles.Get();
    }

    public void ReturnDiamondProjectile(GameObject target)
    {
        gemProjectiles.Release(target);
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

    GameObject MakeJoystick(){
        return joystick = Instantiate(joystickPrefab);
    }

    void DisableJoystick(){
        joystick.SetActive(false);
    }

    void EndStage()
    {
        if(stageEnded){
            return;
        }
        stageEnded = true;
        
        timer.gameObject.SetActive(false);
        Time.timeScale = 0;
        if(joystick){
            DisableJoystick();
        }
        winScreen.SetActive(true);
        GameObject reward = Instantiate(rewardSummaryPrefab);
        reward.transform.SetParent(winScreenSummaryBox);
        reward.transform.localScale = Vector3.one;

        LocalizeStringEvent rewardLocalizeEvent = reward.GetComponentInChildren<LocalizeStringEvent>();
        rewardLocalizeEvent.StringReference.Add("gemsCollected", new IntVariable { Value = gemsCollectedInStage });
        rewardLocalizeEvent.SetTable("TreasureRoom");
        rewardLocalizeEvent.SetEntry("TotalDisplay");
        rewardLocalizeEvent.RefreshString();

        AdvanceRun();
        GameManager.Instance.CollectGems(gemsCollectedInStage);
    }

    public void GemPickup(int gemValue){
        gemsCollectedInStage += gemValue;
        GameManager.Instance.UpdateGemSourceData(GemSources.Treasure, gemValue);
    }

    public void GoToMap()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MapSelection");
    }

    public void AdvanceRun()
    {
        GameManager.Instance.AdvanceMapProgress();
    }
}
