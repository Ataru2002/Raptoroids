using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class InfestedBossBehaviour : BossBehavior
{
    // Start is called before the first frame update
    [SerializeField] ProjectileSpawner leftSpawner1;
    [SerializeField] ProjectileSpawner leftSpawner2;
    [SerializeField] ProjectileSpawner rightSpawner1;
    [SerializeField] ProjectileSpawner rightSpawner2;
    [SerializeField] GameObject bombPrefab;
    [SerializeField] float bombSpeed;
    [SerializeField] float spawnInterval = 5f;
    public bool strafing = false;
    float remainingHealthRatio = 1f;
    bool spawnFromRight;
    [SerializeField] float Phase3BerserkCountDown = 10f; 
    private float spawnTimer = 0;
    void Awake(){
        transitionConditions = new List<System.Func<bool>>{StateTransition1, StateTransition2};
    }

    public void UpdateHealthRatio(float val)
    {
        remainingHealthRatio = val;
        TryTransition();
    }

    bool StateTransition1(){
        if (remainingHealthRatio > 0.7f){   //as long as health is greater than 70%, dont do anything
            return false;
        }
        trackPlayer = true;
        stateExecute = State2Execute;

        return true;
    }

    bool StateTransition2(){
        if (remainingHealthRatio > 0.4f){
            return false;
        }
        gameObject.GetComponent<PlayerAbility>().enabled = true;
        SpawnBomb();
        GetComponent<BoxCollider2D>().enabled = false;
        stateExecute = State3Execute;
        return true;
    }
    void State2Execute(){
        leftSpawner1.TryShoot();
        leftSpawner2.TryShoot();
        rightSpawner1.TryShoot();
        rightSpawner2.TryShoot();
        
    }

    void State3Execute(){
        //spawn bomb objs
        State2Execute();    
        
        spawnTimer += Time.deltaTime;

        if(spawnTimer >= spawnInterval){
            spawnTimer = 0f;
            SpawnBomb();
        }
            
        //if player dont break shield under some time limit, increase fire power dramatically

        
    }
    void SpawnBomb(){
        Vector3 spawnPos;
        spawnFromRight = Random.Range(0, 2) == 0;
        if(spawnFromRight){
            spawnFromRight = true;
            spawnPos = Camera.main.ViewportToWorldPoint(new Vector3(1f, Random.Range(0.7f, 0.9f), 1));
        }
        else{
            spawnFromRight = false;
            spawnPos = Camera.main.ViewportToWorldPoint(new Vector3(0f, Random.Range(0.7f, 0.9f), 1));
        }
        GameObject prefabClone = Instantiate(bombPrefab, spawnPos, Quaternion.identity);
        Rigidbody2D rb = prefabClone.GetComponent<Rigidbody2D>();
        rb.velocity =  spawnFromRight ? Vector2.left * bombSpeed : Vector2.right * bombSpeed;    
    }
    
    public void enableCollider(){
        GetComponent<BoxCollider2D>().enabled = true;
    }
}
