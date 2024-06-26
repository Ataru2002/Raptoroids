using GameAnalyticsSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.Serialization;
using System.Globalization;

public class PlayerAgeInput : MonoBehaviour
{
    public static readonly int ageOfConsent = 16;

    readonly int currentDay = DateTime.Now.Day;
    readonly int currentMonth = DateTime.Now.Month;
    readonly int currentYear = DateTime.Now.Year;

    int dayInput = 0;
    int monthInput = 0;
    int yearInput = 0;

    string dateString = string.Empty;
    DateTime dateInput = default;

    [SerializeField] TMP_Dropdown yearDropdown;
    [SerializeField] GameObject invalidMsg;
    [SerializeField] InitSceneLoader initControl;

    private void Awake()
    {
        List<string> yearList = new List<string>();
        for (int y = 0; y < 100;  y++)
        {
            yearList.Add((currentYear - y).ToString());
        }

        yearDropdown.AddOptions(yearList);

        dayInput = currentDay;
        monthInput = currentMonth;
        yearInput = currentYear;
    }

    public void SetDay(int day)
    {
        // Add 1 because Unity's dropdown options are 0-indexed
        dayInput = day + 1;
        UpdateDateString();
    }

    public void SetMonth(int month)
    {
        // Add 1 because Unity's dropdown options are 0-indexed
        monthInput = month + 1;
        UpdateDateString();
    }

    public void SetYear(int year)
    {
        // Dropdown options count downward from current year, starting at index 0
        yearInput = currentYear - year;
        UpdateDateString();
    }

    public void UpdateDateString()
    {
        string dayPadded = dayInput.ToString().PadLeft(2, '0');
        string monthPadded = monthInput.ToString().PadLeft(2, '0');
        string yearPadded = yearInput.ToString().PadLeft(4, '0');
        dateString = yearInput.ToString() + monthPadded + dayPadded;
        print("dateString updated to " + dateString);
    }

    public void ConfirmInput()
    {
        try
        {
            dateInput = DateTime.ParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture);

            // If player will reach age of consent in the future (i.e., they are not yet at the age of consent)
            if (dateInput.AddYears(ageOfConsent) > DateTime.Today)
            {
                GameAnalytics.SetEnabledEventSubmission(false);
                PlayerPrefs.SetString("PlayerBirthDate", dateString);
                initControl.BeginGame();
            }
            else
            {
                PlayerPrefs.SetString("PlayerBirthDate", dateString);
                initControl.DisplayPrivacyPolicy();
            }

            gameObject.SetActive(false);
        }
        catch (Exception)
        {
            Debug.LogWarning("Player provided invalid date input - got " + dateString);
            invalidMsg.SetActive(true);
        }
    }

    public void SkipAgeInput()
    {
        // Mark player's age as unspecified with an empty string
        PlayerPrefs.SetString("PlayerBirthDate", string.Empty);

        // Comply with Google's requirement to not collect data from players whose age is unknown
        GameAnalytics.SetEnabledEventSubmission(false);

        initControl.BeginGame();
    }
}
