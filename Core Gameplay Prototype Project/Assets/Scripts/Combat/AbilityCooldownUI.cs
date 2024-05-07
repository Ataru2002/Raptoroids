using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownUI : MonoBehaviour
{
    RectTransform cooldownBarRect;
    Image cooldownBarImg;

    [SerializeField] Color readyColor = Color.white;
    [SerializeField] Color rechargingColor = Color.gray;

    const float cooldownBarWidth = 18;
    const float cooldownBarHeight = 150;
    
    private void Awake()
    {
        cooldownBarRect = GetComponent<RectTransform>();
        cooldownBarImg = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateCooldownProgress(float cooldownRatio)
    {
        cooldownBarRect.sizeDelta = new Vector2(cooldownBarWidth, cooldownBarHeight * cooldownRatio);
        cooldownBarImg.color = cooldownRatio < 1 ? rechargingColor : readyColor;
    }
}
