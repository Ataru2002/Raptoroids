using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TreasureRoomManager : MonoBehaviour
{

    private static TreasureRoomManager instance;
    public static TreasureRoomManager Instance { get { return instance; } }
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

    // Start is called before the first frame update
    void Start()
    {
        UpdateDisplay(GameManager.Instance.GetCurrentGems());
    }


    public void UpdateDisplay(int currentGems){
        gemText.text = $"Gems Pending: {currentGems}";
    }
}
