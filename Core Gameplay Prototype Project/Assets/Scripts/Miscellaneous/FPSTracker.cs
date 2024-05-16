using GameAnalyticsSDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FPSTracker : MonoBehaviour
{
    List<int> sceneFPS = new List<int>();

    private void Start()
    {
        SceneManager.sceneUnloaded += SendSceneFPSData;
    }

    // Update is called once per frame
    void Update()
    {
        int instantFPS = Mathf.FloorToInt(1 / Time.unscaledDeltaTime);
        sceneFPS.Add(instantFPS);
    }

    public void SendSceneFPSData(Scene current)
    {
        // Do nothing for initialization scene
        if (current.buildIndex == 0)
        {
            return;
        }

        sceneFPS.Sort();
        int medianIndex = sceneFPS.Count / 2;
        int median = sceneFPS[medianIndex];

        int upperBound = Mathf.RoundToInt(median * 1.2f);
        int lowerBound = Mathf.RoundToInt(median * 0.8f);

        int inThreshold = 1;

        for (int i = medianIndex + 1; i < sceneFPS.Count && sceneFPS[i] <= upperBound; i++)
        {
            inThreshold += 1;
        }

        for (int j = medianIndex - 1; j >= 0 && sceneFPS[j] >= lowerBound; j--)
        {
            inThreshold += 1;
        }

        float stability = inThreshold / (float)sceneFPS.Count;

        GameAnalytics.NewDesignEvent($"FPS:Median:{current.name}", median);
        GameAnalytics.NewDesignEvent($"FPS:Stability:{current.name}", stability);

        sceneFPS.Clear();
    }
}
