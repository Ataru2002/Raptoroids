using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System;
using TMPro;
using Google.MiniJSON;
using UnityEngine.Rendering.PostProcessing;




public class QuestGetter : MonoBehaviour
{
    public string ID;
    public CompQuest dscurrent;
    public TimeReset tscurrent;
    DatabaseReference dbRef;
    public string target;

    private void Awake()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        //LoadData();
    }

    public void SaveData(string ID)
    {
        //Debug.Log(dscurrent.title + " " + dscurrent.progress + " " + dscurrent.goal);
        string json = JsonUtility.ToJson(dscurrent);
        dbRef.Child("Admin").Child(ID).SetRawJsonValueAsync(json);
    }

    public void SaveDate(DateTime insert)
    {
        tscurrent.lastSave = insert.ToString("yyyy-MM-dd HH:mm:ss");
        string json = JsonUtility.ToJson(tscurrent);
        dbRef.Child("Admin").Child("TimeStamp").SetRawJsonValueAsync(json);
    }

    public void LoadData(string ID)
    {
        StartCoroutine(LoadDataEnum(ID));
    }

    public void LoadDate()
    {
        StartCoroutine(LoadDateEnum());
    }

    IEnumerator LoadDataEnum(string ID)
    {
        var serverData = dbRef.Child("Admin").Child(ID).GetValueAsync();
        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        Debug.Log("data: process is complete");

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null)
        {
            Debug.Log("server data found");

            dscurrent = JsonUtility.FromJson<CompQuest>(jsonData);
            Debug.Log("In QuestGetter: " + dscurrent.title + " " + dscurrent.progress + " " + dscurrent.goal);
        }
        else
        {
            Debug.Log("no data found");
        }
    }

    IEnumerator LoadDateEnum()
    {
        var serverData = dbRef.Child("Admin").Child("TimeStamp").GetValueAsync();
        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        Debug.Log("date: process is complete");

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null)
        {
            Debug.Log("server data found");

            tscurrent = JsonUtility.FromJson<TimeReset>(jsonData);
            Debug.Log("In QuestGetter: " + dscurrent.title + " " + dscurrent.progress + " " + dscurrent.goal);
        }
        else
        {
            Debug.Log("no data found");
        }
    }
}
