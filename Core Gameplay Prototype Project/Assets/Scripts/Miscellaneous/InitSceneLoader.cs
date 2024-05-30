using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitSceneLoader : MonoBehaviour
{
    [SerializeField] GameObject privacyPolicyDisplay;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("PrivacyPolicyAccepted"))
        {
            PlayerPrefs.SetInt("PrivacyPolicyAccepted", 0);
        }

        if (PlayerPrefs.GetInt("PrivacyPolicyAccepted") == 0 && Application.platform == RuntimePlatform.Android)
        {
            DisplayPrivacyPolicy();
        }
        else
        {
            BeginGame();
        }
    }

    void DisplayPrivacyPolicy()
    {
        privacyPolicyDisplay.SetActive(true);
    }

    public void BeginGame()
    {
        PlayerPrefs.SetInt("PrivacyPolicyAccepted", 1);
        SceneManager.LoadScene(1);
    }

    public void GoToPrivacyPolicySite()
    {
        Application.OpenURL("https://sirapatp257.github.io/RaptoroidsPrivacyPolicy/");
    }
}
