using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class CompQuest
{
    public string title;
    public int progress;
    public int goal;
}
public class QuestAdder : MonoBehaviour
{
    DatabaseReference dbRef;
    public CompQuest ds;
    public TMP_InputField QuestID;
    public TMP_InputField titlein;
    public TMP_InputField goalin;
    private void Awake()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void SaveData()
    {
        ds.title = titlein.text;
        ds.goal = Int32.Parse(goalin.text);
        ds.progress = 0;
        string json = JsonUtility.ToJson(ds);
        dbRef.Child("Admin").Child(QuestID.text).SetRawJsonValueAsync(json);

    }

    public void testData()
    {
        QuestGetter questGetter = GetComponent<QuestGetter>();
        Debug.Log(questGetter.dscurrent.title + " " + questGetter.dscurrent.progress + " " + questGetter.dscurrent.goal);
    }
}
