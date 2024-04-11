using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfestedBossBehaviour : BossBehavior
{
    // Start is called before the first frame update
    [SerializeField] ProjectileSpawner leftSpawner1;
    [SerializeField] ProjectileSpawner leftSpawner2;
    [SerializeField] ProjectileSpawner rightSpawner1;
    [SerializeField] ProjectileSpawner rightSpawner2;
    [SerializeField] GameObject bombPrefab;

    public bool strafing = false;

    float remainingHealthRatio = 1f;

    void Awake(){
        transitionConditions = new List<System.Func<bool>>{StateTransition1, StateTransition2};
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    bool StateTransition1(){
        if (remainingHealthRatio > 0.7f){   //as long as health is greater than 70%, dont do anything
            return false;
        }

        stateExecute = State2Execute;

        return true;
    }

    bool StateTransition2(){
        return true;
    }
    void State2Execute(){
        leftSpawner1.TryShoot();
        leftSpawner2.TryShoot();
        rightSpawner1.TryShoot();
        rightSpawner2.TryShoot();
        trackPlayer = strafing;
        if(!trackPlayer){
            transform.rotation = Quaternion.Euler(0, 0, 270);
        }
    }

    void State3Execute(){
        //spawn bomb objs

    }
}
