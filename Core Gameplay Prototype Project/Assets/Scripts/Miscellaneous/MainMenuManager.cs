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
        UpdateDisplay(GameManager.Instance.getCurrentGems());
    }

    // Update is called once per frame

    public void UpdateDisplay(int currentGems){
        gemText.text = $"Gems: {currentGems}";
    }
}
