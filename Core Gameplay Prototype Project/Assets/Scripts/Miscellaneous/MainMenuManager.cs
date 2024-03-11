using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class MainMenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static MainMenuManager instance;
    public static MainMenuManager Instance { get { return instance; } }

    [SerializeField] LocalizeStringEvent gemCounterLocalizeEvent;

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

    public void UpdateGemCount()
    {
        gemCounterLocalizeEvent.StringReference.Add("currency", new IntVariable { Value = GameManager.Instance.GetTotalGems() });
        gemCounterLocalizeEvent.StringReference.RefreshString();
    }
}
