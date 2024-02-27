using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    public TMP_Text healthText;
    
    // Start is called before the first frame update
    void Start()
    {
        healthText.text = "Ship's Health: 5";
    }

    // Update is called once per frame
    public void updateHealth(int health)
    {
        string text = "Ship's Health: ";
        text += health.ToString();
        healthText.text = text;
    }
}
