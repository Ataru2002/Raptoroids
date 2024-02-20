using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonDropdownSetup : MonoBehaviour
{
    [SerializeField] TMP_Dropdown sizeDropdown;
    [SerializeField] TMP_Dropdown colorDropdown;

    // Start is called before the first frame update
    void Start()
    {
        int sizeIndex = PlayerPrefs.HasKey("btnSizeIndex") ? PlayerPrefs.GetInt("btnSizeIndex") : 0;
        int colorIndex = PlayerPrefs.HasKey("btnColorIndex") ? PlayerPrefs.GetInt("btnColorIndex") : 0;

        sizeDropdown.SetValueWithoutNotify(sizeIndex);
        colorDropdown.SetValueWithoutNotify(colorIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
