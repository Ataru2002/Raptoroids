using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class SettingsControl : MonoBehaviour
{
    [SerializeField] TMP_Dropdown localeDropdown;
    [SerializeField] Toggle grayScaleMode;

    MonochromeControl monochromeController;

    // Start is called before the first frame update
    void Start()
    {
        monochromeController = FindFirstObjectByType<MonochromeControl>();
    }

    private void OnEnable()
    {
        if (!PlayerPrefs.HasKey("Grayscale"))
        {
            PlayerPrefs.SetInt("Grayscale", 0);
        }
        grayScaleMode.isOn = PlayerPrefs.GetInt("Grayscale") != 0;

        if (!PlayerPrefs.HasKey("LocaleIntID"))
        {
            PlayerPrefs.SetInt("LocaleIntID", 0);
        }
        localeDropdown.value = PlayerPrefs.GetInt("LocaleIntID");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateLocale(int id)
    {
        GameManager.Instance.SetLocale(id);
    }

    public void ToggleGrayscale(bool val)
    {
        PlayerPrefs.SetInt("Grayscale", val ? 1 : 0);
        if (monochromeController != null)
        {
            monochromeController.Refresh();
        }
    }

    public void ResetTutorial()
    {
        PlayerPrefs.SetInt("TutorialComplete", 0);
    }
}
