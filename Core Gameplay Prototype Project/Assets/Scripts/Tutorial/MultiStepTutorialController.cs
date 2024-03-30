using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class MultiStepTutorialController : MonoBehaviour
{
    [SerializeField] string[] subheaderKeys;
    [SerializeField] string[] bodyKeys;
    [SerializeField] GameObject[] highlighters;

    [SerializeField] Button reverseButton;
    [SerializeField] Button advanceButton;

    [SerializeField] LocalizeStringEvent subheader;
    [SerializeField] LocalizeStringEvent body;

    int currentSlide = 0;

    private void OnEnable()
    {
        ResetTutorial();
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetTutorial();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSlide(int direction)
    {
        currentSlide += direction;
        UpdateSlides();
    }

    void ResetTutorial()
    {
        currentSlide = 0;
        UpdateSlides();
    }

    void UpdateSlides()
    {
        subheader.SetEntry(subheaderKeys[currentSlide]);
        subheader.RefreshString();

        body.SetEntry(bodyKeys[currentSlide]);
        body.RefreshString();

        for (int i = 0; i < highlighters.Length; i++)
        {
            highlighters[i].SetActive(i == currentSlide);
        }

        reverseButton.interactable = currentSlide > 0;
        advanceButton.interactable = currentSlide < subheaderKeys.Length - 1;
    }
}
