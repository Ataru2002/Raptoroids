using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    RectTransform healthBarRect;
    Image healthBarImg;

    [SerializeField] Color nominalColor;
    [SerializeField] Color warningColor;
    [SerializeField] Color criticalColor;

    const float healthBarWidth = 18;
    const float healthBarHeight = 150;

    private void Awake()
    {
        healthBarRect = GetComponent<RectTransform>();
        healthBarImg = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateHealth(float healthRatio)
    {
        healthBarRect.sizeDelta = new Vector2(healthBarWidth, healthBarHeight * healthRatio);

        if (healthRatio > 0.6f)
        {
            healthBarImg.color = nominalColor;
        }
        else if (healthRatio > 0.3f)
        {
            healthBarImg.color = warningColor;
        }
        else
        {
            healthBarImg.color = criticalColor;
        }
    }
}
