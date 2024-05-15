using GameAnalyticsSDK;
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
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Gem", gemSourceDict[GemSources.Combat], "Collected", "EnemyDrop");
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Gem", gemSourceDict[GemSources.Treasure], "Collected", "Treasure");
        
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Gem", failurePenalty, "Penalized", "Failure");
    }
}

public enum GemSources
{
    Combat,
    Treasure
}
