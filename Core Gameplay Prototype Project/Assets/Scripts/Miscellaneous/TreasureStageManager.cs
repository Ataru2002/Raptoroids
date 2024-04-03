using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using TMPro;
using System;


public class TreasureStageManager : MonoBehaviour
{
    int gemsCollectedInStage = 0;
    static TreasureStageManager instance;
    public static TreasureStageManager Instance { get { return instance; } }

    LinkedPool<GameObject> diamondProjectiles;
    GameObject diamondPrefab;
    [SerializeField] Transform playerSpawnPoint;
    [SerializeField] GameObject[] playerPrefabs;
    [SerializeField] GameObject winScreen;
    [SerializeField] RectTransform winScreenSummaryBox;
    [SerializeField] TextMeshProUGUI timer;
    [SerializeField] float timeLimit = 10;
    float currentTime = 0;

    public string[] scriptsToDisable;
    GameObject rewardSummaryPrefab;
    

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

            
            rewardSummaryPrefab = Resources.Load<GameObject>("Prefabs/UI Elements/StageSummaryItem");
            diamondPrefab = Resources.Load<GameObject>("Prefabs/Combat Objects/Pickups/Diamond");

        }
    }
    void Start()
    {
        int raptoroidID = PlayerPrefs.HasKey("EquippedRaptoroid") ? PlayerPrefs.GetInt("EquippedRaptoroid") : 0;
        GameObject player = Instantiate(playerPrefabs[raptoroidID]);
        player.transform.position = playerSpawnPoint.position;
    
        diamondProjectiles = new LinkedPool<GameObject>(MakeGemProjectile, OnGetFromPool, OnReleaseToPool, OnPoolItemDestroy, false, 100);
        
    }

    void Update(){
        
        currentTime += Time.deltaTime;

        if(currentTime >= timeLimit){
            EndStage();
        }

        timer.text = Mathf.CeilToInt(timeLimit - currentTime).ToString();
    }


    GameObject MakeGemProjectile(){
        return Instantiate(diamondPrefab);
    }

    public GameObject GetDiamondProjectile()
    {
        return diamondProjectiles.Get();
    }

    public void ReturnDiamondProjectile(GameObject target)
    {
        diamondProjectiles.Release(target);
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

    void EndStage()
    {
        if(stageEnded){
            return;
        }
        stageEnded = true;
        
        timer.gameObject.SetActive(false);
        Time.timeScale = 0;
        winScreen.SetActive(true);
        GameObject reward = Instantiate(rewardSummaryPrefab);
        reward.transform.SetParent(winScreenSummaryBox);
        reward.transform.localScale = Vector3.one;
        reward.GetComponentInChildren<TextMeshProUGUI>().text = gemsCollectedInStage.ToString();
        AdvanceRun();
        GameManager.Instance.CollectGems(gemsCollectedInStage);
    }

    public void GemPickup(int gemValue){
        gemsCollectedInStage += gemValue;
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
