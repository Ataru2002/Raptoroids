using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSpawner : MonoBehaviour
{   
    [SerializeField] float gemMinSpeed = 2.5f;
    [SerializeField] float gemMaxSpeed = 5.0f;
    [SerializeField] float minX = -2.5f;
    [SerializeField] float maxX = 2.5f;
    [SerializeField] float spawnY = 5f;
    [SerializeField] float[] gemSpawnIntervals = {0.25f, 0.5f, 1f};
    [SerializeField] DropTableEntry<GemProjectileData>[] gemData;
    
    void Start()
    {
        // Assume [Diamond, Emerald, Ruby, Sapphire] for weight shifting
        int modifier = Mathf.Clamp(GameManager.Instance.EnemiesSinceLastTreasureRoom, 0, 50);

        gemData[0].weight += 0.002f * modifier;
        gemData[1].weight += 0.003f * modifier;
        gemData[2].weight += 0.005f * modifier;
        gemData[3].weight -= 0.01f * modifier;

        print($"Diamond chance: {gemData[0].weight}\nRuby chance: {gemData[1].weight}\nSapphire chance: {gemData[2].weight}\nEmerald chance: {gemData[3].weight}");

        GameManager.Instance.EnemiesSinceLastTreasureRoom = 0;

        StartCoroutine(spawnGems());
    }

    private IEnumerator spawnGems(){
        while(true){
            Vector2 spawnPos = new Vector2(Random.Range(minX, maxX), spawnY);
            GameObject gem = TreasureStageManager.Instance.GetDiamondProjectile();
            gem.transform.position = spawnPos;
            gem.GetComponent<GemProjectile>().speed = Random.Range(gemMinSpeed, gemMaxSpeed);
            
            GemProjectileData randGemData = gemData[gemData.Length - 1].item;
            float gemSelector = Random.value;
            for (int i = 0; i < gemData.Length; i++)
            {
                gemSelector -= gemData[i].weight;
                if (gemSelector <= 0)
                {
                    randGemData = gemData[i].item;
                    break;
                }
            }

            gem.GetComponent<SpriteRenderer>().sprite = randGemData.gemSprite;
            gem.GetComponent<GemCollectible>().SetValue(randGemData.gemValue);
            
            int rngInterval = Random.Range(0, gemSpawnIntervals.Length);
            yield return new WaitForSeconds(gemSpawnIntervals[rngInterval]);
        }
    }
    
}
