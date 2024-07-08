using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using GameAnalyticsSDK;

public class TutorialRoomManager : MonoBehaviour
{
    private static TutorialRoomManager instance;
    public static TutorialRoomManager Instance { get { return instance; } }
    [SerializeField] GameObject raptoroidPrefab;
    [SerializeField] WeaponData tutorialWeaponData;

    [SerializeField] GameObject enemyPrefab;
    [SerializeField] TextMeshProUGUI killCounter;
    [SerializeField] GameObject introduction;
    [SerializeField] GameObject transition1;
    [SerializeField] GameObject transition2;
    [SerializeField] GameObject transition3;
    [SerializeField] GameObject transition4;
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject loseScreen;
    LinkedPool<GameObject> playerProjectiles;
    [SerializeField] GameObject playerProjectilePrefab;
    LinkedPool<GameObject> enemyProjectiles;
    [SerializeField] GameObject hitParticlesPrefab;
    LinkedPool<GameObject> enemyHitParticles;
    [SerializeField] GameObject enemyProjectilePrefab;
    // [SerializeField] ProjectileSpawner playerProjectileSpawner;
    GameObject enemy;
    GameObject raptoroid;   
    int boxCollided = 0;
    int enemyDefeated = 0;

    void Awake(){
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else{
            instance = this;
            playerProjectiles = new LinkedPool<GameObject>(MakePlayerProjectile, OnGetFromPool, OnReleaseToPool, OnPoolItemDestroy, false, 1000);
            enemyProjectiles = new LinkedPool<GameObject>(MakeEnemyProjectile, OnGetFromPool, OnReleaseToPool, OnPoolItemDestroy, false, 1000);
            enemyHitParticles = new LinkedPool<GameObject>(MakeEnemyParticles, OnGetFromPool, OnReleaseToPool, OnPoolItemDestroy, false, 1000);
        }
    }

    void Start()
    {
        PlayerPrefs.SetInt("TutorialComplete", 0);
        GameAnalytics.NewDesignEvent("Tutorial:Start");
        IntroductionBehaviour.onIntroduction += IntroductionOver;
        
        raptoroid = Instantiate(raptoroidPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        ProjectileSpawner tutorialWeapon = raptoroid.GetComponentInChildren<ProjectileSpawner>();
        tutorialWeapon.SetWeaponData(tutorialWeaponData);
        tutorialWeapon.enabled = false;
    }

    GameObject MakePlayerProjectile()
    {
        return Instantiate(playerProjectilePrefab);
    }

    GameObject MakeEnemyProjectile(){
        return Instantiate(enemyProjectilePrefab);
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

    public GameObject GetPlayerProjectile(){
        return playerProjectiles.Get();
    }

    public GameObject GetEnemyProjectile(){
        return enemyProjectiles.Get();
    }

    public void ReturnEnemyProjectile(GameObject target)
    {
        enemyProjectiles.Release(target);
    }
    public void ReturnPlayerProjectile(GameObject target)
    {
        playerProjectiles.Release(target);
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

    private void IntroductionOver(){
        introduction.SetActive(false);
        transition1.SetActive(true);
        
        IntroductionBehaviour.onIntroduction -= IntroductionOver;
        WhiteheadAbility.onShieldActivated += Transition1Over;
    }

    private void Transition1Over(){
        
        transition1.SetActive(false);
        transition2.SetActive(true);
        WhiteheadAbility.onShieldActivated -= Transition1Over;
        WhiteheadAbility.onShieldCDFull += Transition2Over;
    }

    private void Transition2Over(){
        
        transition2.SetActive(false);
        transition3.SetActive(true);
        WhiteheadAbility.onShieldCDFull -= Transition2Over;
        raptoroid.GetComponentInChildren<ProjectileSpawner>().enabled = true;
        enemy = Instantiate(enemyPrefab, new Vector3(0, 3, 0), Quaternion.Euler(0, 0, 270));
        Vector2[] path = new Vector2[2]{new Vector2(0, 5), new Vector2(0, 3)};
        enemy.GetComponent<EnemyBehavior>().SetPath(path);
        enemy.GetComponent<EnemyHealth>().OnDefeat.AddListener(Transition3Over);
        
    }

    private void Transition3Over(){
        enemyDefeated += 1;
        killCounter.text = $"{enemyDefeated} / 1";
        transition4.SetActive(true);
        enemy.GetComponent<EnemyHealth>().OnDefeat.RemoveAllListeners();
        transition3.SetActive(false);
        StartCoroutine(endTutorial(5));        
    }

    IEnumerator endTutorial(float seconds){
        yield return new WaitForSeconds(seconds);
        transition4.SetActive(false);
        winScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void toMap(){
        Time.timeScale = 1;
        GameManager.Instance.AdvanceMapProgress();
        SceneManager.LoadScene("MapSelection");
    }

    public void OnPlayerDefeated(){
        loseScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void RetryTutorial()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
