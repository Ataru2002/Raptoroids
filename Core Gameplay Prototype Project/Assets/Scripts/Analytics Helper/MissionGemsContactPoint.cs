using GameAnalyticsSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionGemsContactPoint : MonoBehaviour
{
    Dictionary<GemSources, int> gemSourceDict;
    
    int failurePenalty;

    private void Start()
    {
        gemSourceDict = new Dictionary<GemSources, int>();
        ResetData();
    }

    public void ResetData()
    {
        foreach (GemSources sourceType in Enum.GetValues(typeof(GemSources)))
        {
            gemSourceDict[sourceType] = 0;
        }

        failurePenalty = 0;
    }

    public void SetSourceData(GemSources sourceType, int delta)
    {
        gemSourceDict[sourceType] += delta;
    }

    public void SetPenaltyValue(int value)
    {
        failurePenalty = value;
    }

    public void SendData()
    {
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Gem", gemSourceDict[GemSources.Combat], "Reward", "EnemyDrop");
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Gem", gemSourceDict[GemSources.Treasure], "Reward", "Treasure");
        
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Gem", failurePenalty, "Penalty", "Failure");
    }
}

public enum GemSources
{
    Combat,
    Treasure
}
