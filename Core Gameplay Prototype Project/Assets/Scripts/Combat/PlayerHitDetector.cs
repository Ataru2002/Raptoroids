using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitDetector : MonoBehaviour, IBulletHittable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBulletHit()
    {
        print("Player hit by an enemy bullet!");

        // TODO: implement some form of health system
        if (CombatStageManager.Instance != null)
        {
            gameObject.SetActive(false);
            CombatStageManager.Instance.OnPlayerDefeated();
        }
    }
}
