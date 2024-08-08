using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InfestedBossBehaviour : BossBehavior
{
    // Start is called before the first frame update
    [SerializeField] ProjectileSpawner leftSpawner1;
    [SerializeField] ProjectileSpawner leftSpawner2;
    [SerializeField] ProjectileSpawner rightSpawner1;
    [SerializeField] ProjectileSpawner rightSpawner2;
    [SerializeField] TentacleBehavior[] tentacles;
    [SerializeField] StrafeEnemyBehavior strafeBehavior;
    [SerializeField] GameObject bombPrefab;
    [SerializeField] GameObject hostBody;
    [SerializeField] float bombSpeed;
    [SerializeField] float spawnInterval = 5f;

    Vector3 originalPosition;
    bool tentacleAttacking = false;
    public bool strafing = false;
    float remainingHealthRatio = 1f;
    bool spawnFromRight;
    private float spawnTimer = 5f;
    bool firstHintDisplayed = false;

    new void Awake(){
        base.Awake();
        transitionConditions = new List<System.Func<bool>>{StateTransition1, StateTransition2, StateTransition3};
        CombatStageManager.Instance.SetBossHintTable("ParasiteHints");
        CombatStageManager.Instance.RegisterEnemyTransform(hostBody.transform);
        stateExecute = State1Execute;
    }

    void State1Execute(){
        if (!strafing) {
            strafeBehavior.enabled = true;
            strafing = true;
        }

        if(!firstHintDisplayed){
            DisplayFirstHint();
        }
    }

    public void UpdateHealthRatio(float val)
    {
        remainingHealthRatio = val;
        TryTransition();
    }

    bool StateTransition1(){
        if(remainingHealthRatio > 0.9){
            return false;
        }
        defaultWeapon.enabled = false;
        
        stateExecute = State2Execute; 

        return true;
    }

    void State2Execute(){
        leftSpawner1.TryShoot();
        leftSpawner2.TryShoot();
        rightSpawner1.TryShoot();
        rightSpawner2.TryShoot();
        State1Execute();
    }

    bool StateTransition2(){
        if (remainingHealthRatio > 0.7f){
            return false;
        }
        gameObject.GetComponent<WhiteheadAbility>().enabled = true;
        hostBody.GetComponent<BoxCollider2D>().enabled = false;
        trackPlayer = true;

        ParasiteManager.Instance.SetShieldStatus(true);

        CombatStageManager.Instance.DisplayBossHint("hint02");

        stateExecute = State3Execute;

        return true;
    }

    void State3Execute(){
        leftSpawner1.TryShoot();
        leftSpawner2.TryShoot();
        rightSpawner1.TryShoot();
        rightSpawner2.TryShoot();
        spawnTimer += Time.deltaTime;
         
        if((spawnTimer >= spawnInterval) && ParasiteManager.Instance.BossShieldStatus()){
            SpawnBomb();
            spawnTimer = 0f;
        }

        // TODO: maybe fire an event when the shield breaks instead so we don't have to
        // run this check every frame?
        if(!ParasiteManager.Instance.BossShieldStatus()){
            hostBody.GetComponent<BoxCollider2D>().enabled = true;
            defaultWeapon.enabled = true;
        }
    }

    bool StateTransition3(){
        if(remainingHealthRatio > 0.3f){
            return false;
        }
        
        hostBody.SetActive(false);
        originalPosition = finalPosition;
        transform.position = originalPosition;
        transform.rotation = Quaternion.Euler(0, 0, 270);
        
        EnableTentacleHitbox();
        trackPlayer = false;
        strafeBehavior.enabled = false;

        foreach (TentacleBehavior tentacle in tentacles)
        {
            CombatStageManager.Instance.RegisterEnemyTransform(tentacle.transform);
        }

        CombatStageManager.Instance.DisplayBossHint("hint03", 2);
        stateExecute = State4Execute;
        return true;
    }

    void State4Execute(){
        if(!tentacleAttacking){
            foreach(TentacleBehavior tentacle in tentacles){
                //tentacle.TryAttackSequenceIndicator();
                if(tentacle != null){
                    tentacle.TryTentacleSequence();
                }
                
            }
        }
    }

    void DisplayFirstHint(){
        CombatStageManager.Instance.DisplayBossHint("hint01");
        firstHintDisplayed = true;
    }

    void SpawnBomb(){
        Vector3 spawnPos;
        spawnFromRight = Random.Range(0, 2) == 0;
        if(spawnFromRight){
            spawnPos = Camera.main.ViewportToWorldPoint(new Vector3(1f, Random.Range(0.7f, 0.9f), 1));
        }
        else{
            spawnPos = Camera.main.ViewportToWorldPoint(new Vector3(0f, Random.Range(0.7f, 0.9f), 1));
        }
        GameObject prefabClone = Instantiate(bombPrefab, spawnPos, Quaternion.identity);
        Rigidbody2D rb = prefabClone.GetComponent<Rigidbody2D>();
        rb.velocity =  spawnFromRight ? Vector2.left * bombSpeed : Vector2.right * bombSpeed;   
    }

    private void EnableTentacleHitbox(){
        foreach(TentacleBehavior tentacle in tentacles){
            if(tentacle != null){
                tentacle.GetComponent<BoxCollider2D>().enabled = true;
            }
        }
    }
}
