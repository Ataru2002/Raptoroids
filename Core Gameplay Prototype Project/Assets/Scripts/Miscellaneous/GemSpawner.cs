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
