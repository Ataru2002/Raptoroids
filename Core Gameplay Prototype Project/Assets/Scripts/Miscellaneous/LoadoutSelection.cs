using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadoutSelection : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] TMP_Text Ship1;
    [SerializeField] TMP_Text Ship2;

    void Start()
    {
        
    }

    public void EquipRaptoroid(int type)
    {
        PlayerPrefs.SetInt("EquippedRaptoroid", type);
        if (type == 0)
        {
            Ship1.text = "Equipped";
            Ship2.text = "Equip";
        }
        else
        {
            Ship2.text = "Equipped";
            Ship1.text = "Equip";
        }
    }
}
