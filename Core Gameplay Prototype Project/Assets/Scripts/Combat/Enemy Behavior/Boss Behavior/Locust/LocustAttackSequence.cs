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
    [SerializeField] MantisBodyBehaviour body;
    float remainingHealthRatio = 1f;
    private bool swarmBarrageStarted = false;
    private bool swarmFusionDone = false;
    bool firstHintDisplayed = false;
    bool spawningGiantBoss = false;
    bool boomerangBarrageStarted = false;
    bool invisAttackStarted = false;
    public int miniEnemiesDestroyed;
    
    new void Awake()
    {
        transitionConditions = new List<System.Func<bool>>
        {
            StateTransition1,
            StateTransition2
        };

        CombatStageManager.Instance.SetBossHintTable("MantisHints");
        foreach (SwarmBehaviour mini in miniLocustEnemy)
        {
            CombatStageManager.Instance.RegisterEnemyTransform(mini.transform);
        }

        stateExecute = StateDefaultExecute;
    }

    public void UpdateHealthRatio(float val)
    {
        remainingHealthRatio = val;
        TryTransition();
    }

    #region STATE TRANSITIONS
    bool StateTransition1(){
        if(remainingHealthRatio > 0.7f){
            return false;
        }
        
        SwarmFusion();
        SwarmBehaviour.onFusionComplete += spawnGiantBoss;
        
        stateExecute = State1Execute;
        return true;
    }

    bool StateTransition2(){
        if(remainingHealthRatio > 0.3f){
            return false;
        }
        CombatStageManager.Instance.DisplayBossHint("hint03");
        CombatStageManager.Instance.RegisterEnemyTransform(giantBoss.transform);

        stateExecute = State2Execute;
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
        if(!firstHintDisplayed){
            DisplayFirstHint();
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

    void State2Execute(){
        if(invisAttackStarted){
            return;
        }

        if(!invisAttackStarted){
            invisAttack();
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
            boomerangBarrageStarted = true;
        }
        if(giantRightArm != null){
            giantRightArm.tryBoomerangAttack();
            boomerangBarrageStarted = true;
        }
        boomerangBarrageStarted = false;
    }
    private void invisAttack(){
        invisAttackStarted = true;
        body.enabled = true;
        body.TryInvisAttack();

        invisAttackStarted = false;
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

        fusionBall.transform.localScale = new Vector3(1.5f, 1f);
        yield return new WaitForSeconds(1);

        fusionBall.transform.localScale = new Vector3(2f, 1.5f);
        yield return new WaitForSeconds(2);

        fusionBall.SetActive(false);
        giantBoss.SetActive(true);
        CombatSFXManager.PlaySoundAtLocation("FusionBallBreak", giantBoss.transform.position);
        CombatStageManager.Instance.DisplayBossHint("hint02");
        giantBossLeftArm.SetActive(true);
        giantBossRightArm.SetActive(true);
        CombatStageManager.Instance.RegisterEnemyTransform(giantBossLeftArm.transform);
        CombatStageManager.Instance.RegisterEnemyTransform(giantBossRightArm.transform);
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

    void DisplayFirstHint(){
        CombatStageManager.Instance.DisplayBossHint("hint01");
        firstHintDisplayed = true;
    }

    #endregion

    private void OnDestroy()
    {
        SwarmBehaviour.onFusionComplete -= spawnGiantBoss;
    }
}
