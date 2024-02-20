using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum StageResult
{
    Win,
    Lose
}

public class MockActionScene : MonoBehaviour
{
    [SerializeField] GameObject nonBossWinScreen;
    [SerializeField] GameObject bossWinScreen;
    [SerializeField] GameObject loseScreen;
    
    [SerializeField] GameObject bossHUD;

    static MockActionScene instance;
    public static MockActionScene Manager { get { return instance; } }

    bool isBoss = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        isBoss = GameManager.Instance.MapTier >= 4;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayEndScreen(StageResult result)
    {
        if (result == StageResult.Win)
        {
            if (isBoss)
            {
                bossWinScreen.SetActive(true);
            }
            else
            {
                nonBossWinScreen.SetActive(true);
            }
        }
        else
        {
            loseScreen.SetActive(true);
        }
    }

    public void BackToMap()
    {
        GameManager.Instance.AdvanceMapProgress();
        SceneManager.LoadScene("MapSelection");
        ButtonSFXPlayer.Instance.PlaySFX("ToMaps");
    }

    public void ToMainMenu()
    {
        GameManager.Instance.ClearMapInfo();
        SceneManager.LoadScene("MainMenu");
        ButtonSFXPlayer.Instance.PlaySFX("ToMenus");
    }
}
