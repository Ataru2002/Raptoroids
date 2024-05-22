using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocustAttackSequence : BossBehavior
{
    [SerializeField] SwarmBehaviour[] miniLocustEnemy;
    [SerializeField] GameObject giantBoss;
    [SerializeField] GameObject giantBossLeftArm;
    [SerializeField] GameObject giantBossRightArm;
    [SerializeField] GameObject fusionBall;
    [SerializeField] GiantArmBehaviour giantLeftArm;
    [SerializeField] GiantArmBehaviour giantRightArm;
    float remainingHealthRatio = 1f;
    private bool swarmBarrageStarted = false;
    private bool swarmFusionDone = false;
    bool spawningGiantBoss = false;
    bool boomerangBarrageStarted = false;
    public int miniEnemiesDestroyed;
    void Awake()
    {
        transitionConditions = new List<System.Func<bool>>
        {
            StateTransition1
            // StateTransition2
        };

        
        
        stateExecute = StateDefaultExecute;
    }

    public void UpdateHealthRatio(float val)
    {
        remainingHealthRatio = val;
        TryTransition();
    }

    #region STATE TRANSITIONS
    bool StateTransition1(){
        if(remainingHealthRatio > 0.7f || miniEnemiesDestroyed < 3){
            return false;
        }

        SwarmFusion();
        SwarmBehaviour.onFusionComplete += spawnGiantBoss;
        stateExecute = State1Execute;
        
        return true;
    }
    #endregion

    #region STATE EXECUTE
    void StateDefaultExecute(){
        if (!FinalPositionReached)
        {
            return;
        }
        if (!swarmBarrageStarted){
            SwarmBarrage();
        }
    }

    void State1Execute(){
        if(boomerangBarrageStarted || spawningGiantBoss){
            return;
        }

        if(!boomerangBarrageStarted){
            boomerangBarrage();
        }
    }
    #endregion
    
    #region TRY ACTION
    private void SwarmBarrage(){
        swarmBarrageStarted = true;

        for(int i = 0; i < miniLocustEnemy.Length; i++){
            if(miniLocustEnemy[i] != null){
                miniLocustEnemy[i].TrySwarmAttack();
            }
        }
        swarmBarrageStarted = false;
    }
    private void SwarmFusion(){
        swarmFusionDone = false;
        fusionBall.SetActive(true);
        for(int i = 0; i < miniLocustEnemy.Length; i++){
            if(miniLocustEnemy[i] != null){
                miniLocustEnemy[i].TryFusion();
            }
        }
        swarmFusionDone = true;
    }
    private void boomerangBarrage(){
        if(giantLeftArm != null){
            giantLeftArm.tryBoomerangAttack();
        }
        if(giantRightArm != null){
            giantRightArm.tryBoomerangAttack();
        }
    }
    #endregion

    

    void spawnGiantBoss(){
        if(spawningGiantBoss){
            return;
        }
        spawningGiantBoss = true;
        StartCoroutine(metamorph());
    }

    #region COROUTINES
    IEnumerator metamorph(){  
        for(int i = 0; i < miniLocustEnemy.Length; i++){
            while(miniLocustEnemy[i] != null){
                yield return new WaitForEndOfFrame();
            }
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(flicker());
        fusionBall.transform.localScale = new Vector3(2, 1.5f);
        yield return new WaitForSeconds(1);
        fusionBall.transform.localScale = new Vector3(2.5f, 2);
        
        
        yield return new WaitForSeconds(2);
        fusionBall.SetActive(false);
        giantBoss.SetActive(true);
        giantBossLeftArm.SetActive(true);
        giantBossRightArm.SetActive(true);
        spawningGiantBoss = false;
    }
    IEnumerator flicker(){
        float step = 0;
        float stepTime = 0.4f;
        Color color = new Color(0xB4, 0xA8, 0x2F, 1);
        SpriteRenderer spriteRenderer = fusionBall.GetComponent<SpriteRenderer>();
        while(true){
            spriteRenderer.color = step % 2 == 0 ? color : new Color(0xB4, 0xA8, 0x2F, 0.5f);
            yield return new WaitForSeconds(stepTime);
            step++;
        }
    }

    
    #endregion
}
