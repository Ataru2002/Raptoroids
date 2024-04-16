using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MapScreenStatsTextControl : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI hiScoreText;
    [SerializeField] TextMeshProUGUI gemText;

    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = GameManager.Instance.GetCurrentScore().ToString();
        hiScoreText.text = GameManager.Instance.GetHighScore().ToString();
        gemText.text = GameManager.Instance.GetCurrentGems().ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
