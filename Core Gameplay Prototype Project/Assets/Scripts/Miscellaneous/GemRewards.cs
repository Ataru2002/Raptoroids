using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GemRewards : IRewardProcessor
{
    static int small = 10;
    static int medium = 50;
    static int big = 100;

    public TreasureRoomManager gemCollection;
    //change the weights here

    private Dictionary<int, int> gemWeights = new Dictionary<int, int>{
        {small, 65},
        {medium, 25},
        {big, 10}
    };

    public void ProcessReward(){
        int selectedGem = getRandomByWeight();
        GameManager.Instance.collectGems(selectedGem);
        TreasureRoomManager.Instance.UpdateDisplay(GameManager.Instance.getCurrentGems());
    }

    public int getRandomByWeight(){
        int totalWeight = 0;
        foreach (int weight in gemWeights.Values){
            totalWeight += weight;
        }

        int rng = UnityEngine.Random.Range(0, totalWeight);

        foreach (KeyValuePair<int, int> kvp in gemWeights){
            rng -= kvp.Value;
            if(rng < 0){
                return kvp.Key;
            }
        }
        return -1;    
    }

}
