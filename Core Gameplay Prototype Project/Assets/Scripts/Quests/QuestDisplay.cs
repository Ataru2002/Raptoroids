using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestDisplay : MonoBehaviour
{
    public QuestGetter qgetter;
    public TMP_Text questtxt1;
    bool initialLoad = true;
    public Slider slider;
    public TMP_Text progress1;
    public float updateInterval = 0.25f; // interval in second
    private float timer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        qgetter = GetComponent<QuestGetter>();
        qgetter.LoadData("Quest 2");
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            Debug.Log(qgetter.dscurrent.title + " " + qgetter.dscurrent.progress + " " + qgetter.dscurrent.goal); 
            questtxt1.text = qgetter.dscurrent.title;
            float currentProgress = qgetter.dscurrent.progress;
            float currentGoal = qgetter.dscurrent.goal;
            slider.value = (((currentProgress / currentGoal) < 1.0f) ? (currentProgress / currentGoal) : 1.0f);
            progress1.text = qgetter.dscurrent.progress.ToString() + "/" + qgetter.dscurrent.goal.ToString();
            if (initialLoad)
            {
                updateInterval = 3.0f;
                initialLoad = false;
            }
            timer = 0.0f;
        }
    }
}
