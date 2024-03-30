using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class TreasureRoomManager : MonoBehaviour
{

    private static TreasureRoomManager instance;
    public static TreasureRoomManager Instance { get { return instance; } }
    public LocalizeStringEvent gemText;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            gemText.StringReference.Add("collectedGems", new IntVariable { Value = 0 });
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateDisplay(GameManager.Instance.GetCurrentGems());
    }


    public void UpdateDisplay(int currentGems){
        gemText.StringReference.Add("collectedGems", new IntVariable { Value = currentGems });
        gemText.StringReference.RefreshString();
    }
}
