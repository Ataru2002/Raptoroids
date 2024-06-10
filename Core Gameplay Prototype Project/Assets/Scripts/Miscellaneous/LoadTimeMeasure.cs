using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTimeMeasure : MonoBehaviour
{
    float stopwatch = 0;

    private void Awake()
    {
        stopwatch = Time.realtimeSinceStartup;
    }

    public void EnableMeasurement()
    {
        SceneManager.sceneUnloaded += StartWatch;
        SceneManager.sceneLoaded += StopWatch;
    }

    public void StartWatch(Scene unloadedScene)
    {
        stopwatch = Time.realtimeSinceStartup;
    }

    public void StopWatch(Scene loadedScene, LoadSceneMode loadSceneMode)
    {
        print($"Loaded scene {loadedScene.name} in {Time.realtimeSinceStartup - stopwatch} seconds");
    }
}
