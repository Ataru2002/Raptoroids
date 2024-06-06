using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


[Serializable]
public class CompQuest
{
    public string title;
    public int progress;
    public int goal;
}

[Serializable]
public class TimeReset
{
    public string lastSave;
}
public class QuestAdder : MonoBehaviour
{
    DatabaseReference dbRef;
    public CompQuest ds;
    public TimeReset ts;
    public TMP_InputField QuestID;
    public TMP_InputField titlein;
    public TMP_InputField goalin;
    public TMP_Text currentTimeDis;
    public TMP_Text futureTimeDis;
    public DateTime currentTime;
    public DateTime futureTime;
    private float timer = 0.0f;
    private float updateInterval = 1.0f; // interval in second
    private void Awake()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        currentTime = DateTime.Now;
        futureTime = currentTime.AddDays(7);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            currentTime = DateTime.Now;
            futureTime = currentTime.AddDays(7);
            currentTimeDis.text = currentTime.ToString();
            futureTimeDis.text = futureTime.ToString();

            TimeSpan val = futureTime - currentTime;
            //Debug.Log(val.Days);
            timer = 0.0f;
        }
    }

    public void SaveData()
    {
        ds.title = titlein.text;
        ds.goal = Int32.Parse(goalin.text);
        ds.progress = 0;
        string json = JsonUtility.ToJson(ds);
        dbRef.Child("Admin").Child(QuestID.text).SetRawJsonValueAsync(json);

    }

    public void SaveDate()
    {
        ts.lastSave = currentTime.ToString("yyyy-MM-dd HH:mm:ss");
        string json = JsonUtility.ToJson(ts);
        dbRef.Child("Admin").Child("TimeStamp").SetRawJsonValueAsync(json);

    }

    public void testData()
    {
        QuestGetter questGetter = GetComponent<QuestGetter>();
        Debug.Log(questGetter.dscurrent.title + " " + questGetter.dscurrent.progress + " " + questGetter.dscurrent.goal);
    }


}
