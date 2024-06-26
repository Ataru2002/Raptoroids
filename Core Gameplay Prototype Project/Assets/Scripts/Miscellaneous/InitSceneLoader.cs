using GameAnalyticsSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitSceneLoader : MonoBehaviour
{
    [SerializeField] GameObject ageInputPrompt;
    [SerializeField] GameObject privacyPolicyDisplay;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("PlayerBirthDate"))
        {
            DisplayAgePrompt();
        }

        else if (AgeOfConsentCheck())
        {
            if (!PlayerPrefs.HasKey("DataConsent"))
            {
                // Set default to invalid value so that the player must actively
                // provide or deny consent
                PlayerPrefs.SetInt("DataConsent", -1);
                DisplayPrivacyPolicy();
            }
            else
            {
                if (PlayerPrefs.GetInt("DataConsent") < 0)
                {
                    DisplayPrivacyPolicy();
                }
                else
                {
                    bool consent = PlayerPrefs.GetInt("DataConsent") != 0;
                    GameAnalytics.SetEnabledEventSubmission(consent);
                    BeginGame();
                }
            }
        }

        else
        {
            GameAnalytics.SetEnabledEventSubmission(false);
            BeginGame();
        }
    }

    void DisplayAgePrompt()
    {
        ageInputPrompt.SetActive(true);
    }

    bool AgeOfConsentCheck()
    {
        string dobString = PlayerPrefs.GetString("PlayerBirthDate");
        // Treat players of unknown age as if they are underage
        if (dobString == string.Empty)
        {
            return false;
        }

        DateTime dob = DateTime.ParseExact(dobString, "yyyyMMdd", CultureInfo.InvariantCulture);
        return dob.AddYears(PlayerAgeInput.ageOfConsent) <= DateTime.Today;
    }

    public void DisplayPrivacyPolicy()
    {
        privacyPolicyDisplay.SetActive(true);
    }

    public void AcceptDataConsent(bool accept)
    {
        PlayerPrefs.SetInt("DataConsent", accept ? 1 : 0);
        GameAnalytics.SetEnabledEventSubmission(accept);
    }

    public void BeginGame()
    {
        SceneManager.LoadScene(1);
    }

    public void GoToPrivacyPolicySite()
    {
        Application.OpenURL("https://sirapatp257.github.io/RaptoroidsPrivacyPolicy/");
    }
}
