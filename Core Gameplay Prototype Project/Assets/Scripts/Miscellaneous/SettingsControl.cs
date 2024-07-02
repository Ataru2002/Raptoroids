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

    [SerializeField] Toggle sfxToggle;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Toggle joystickToggle;

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

        if (!PlayerPrefs.HasKey("sfxOn"))
        {
            PlayerPrefs.SetInt("sfxOn", 1);
        }
        sfxToggle.isOn = PlayerPrefs.GetInt("sfxOn") != 0;

        if (!PlayerPrefs.HasKey("sfxVol"))
        {
            PlayerPrefs.SetFloat("sfxVol", 1);
        }
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVol") * 100;

        if (!PlayerPrefs.HasKey("LocaleIntID"))
        {
            PlayerPrefs.SetInt("LocaleIntID", 0);
        }
        localeDropdown.value = PlayerPrefs.GetInt("LocaleIntID");
        if (!PlayerPrefs.HasKey("joystick"))
        {
            PlayerPrefs.SetInt("joystick", 0);
        }
        joystickToggle.isOn = PlayerPrefs.GetInt("joystick") != 0;
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
    
    public void ToggleJoystick(bool val)
    {
        PlayerPrefs.SetInt("joystick", val ? 1 : 0);
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

    public void ToggleSFX(bool val)
    {
        PlayerPrefs.SetInt("sfxOn", val ? 1 : 0);
        ButtonSFXPlayer.Instance.UpdateVolume();
    }

    public void SetSFXVolume(float val)
    {
        PlayerPrefs.SetFloat("sfxVol", val / 100f);
        ButtonSFXPlayer.Instance.UpdateVolume();
    }

    public void ResetTutorial()
    {
        PlayerPrefs.SetInt("TutorialComplete", 0);
    }
}
