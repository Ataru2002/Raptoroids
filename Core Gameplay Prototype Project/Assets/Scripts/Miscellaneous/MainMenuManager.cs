using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static MainMenuManager instance;
    public static MainMenuManager Instance { get { return instance; } }
    public TMP_Text gemText;

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

    void Start()
    {
        UpdateGemCount();
    }

    private void OnEnable()
    {
        UpdateGemCount();
    }

    public void UpdateGemCount(){
        gemText.text = $"Gems: {GameManager.Instance.GetTotalGems()}";
    }
}
