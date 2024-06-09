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
    [SerializeField] Toggle bgmToggle;
    [SerializeField] Slider bgmSlider;

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

        if (!PlayerPrefs.HasKey("bgmOn"))
        {
            PlayerPrefs.SetInt("bgmOn", 1);
        }
        bgmToggle.isOn = PlayerPrefs.GetInt("bgmOn") != 0;

        if (!PlayerPrefs.HasKey("bgmVol"))
        {
            PlayerPrefs.SetFloat("bgmVol", 1);
        }
        bgmSlider.value = PlayerPrefs.GetFloat("bgmVol") * 100;

        if (!PlayerPrefs.HasKey("LocaleIntID"))
        {
            PlayerPrefs.SetInt("LocaleIntID", 0);
        }
        localeDropdown.value = PlayerPrefs.GetInt("LocaleIntID");
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

    public void ToggleBGM(bool val)
    {
        PlayerPrefs.SetInt("bgmOn", val ? 1 : 0);
        BGMPlayer bgmPlayer = FindFirstObjectByType<BGMPlayer>();
        if (bgmPlayer != null)
        {
            bgmPlayer.UpdateVolume();
        }
    }

    public void SetBGMVolume(float val)
    {
        PlayerPrefs.SetFloat("bgmVol", val / 100f);
        BGMPlayer bgmPlayer = FindFirstObjectByType<BGMPlayer>();
        if (bgmPlayer != null)
        {
            bgmPlayer.UpdateVolume();
        }
    }

    public void ResetTutorial()
    {
        PlayerPrefs.SetInt("TutorialComplete", 0);
    }
}
