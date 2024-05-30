using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CondoroidAbility : RaptoroidAbility
{
    PlayerHealth playerHealth;
    int rageMeter;
    SpriteRenderer spriteRenderer;
    [SerializeField] GameObject prefab;

    void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        rageMeter = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
        cooldown = 15;
    }

    public void ActivateAbility(){
        if(rageMeter > 0 && cooldownTimer <= 0){
            GameObject spawnBullet = Instantiate(prefab, new Vector3(transform.position.x, transform.position.y+1, 0), Quaternion.identity);
            spawnBullet.GetComponent<BulletBehavior>().MarkNotPool();
            spawnBullet.GetComponent<BulletBehavior>().AllowPunchThrough();
            cooldownTimer = cooldown;
        }
    }

    //Intentionally left blank
    protected override void UpdateAbilityDuration(){
        
        
    }

    protected override void DeactivateAbility(){

    }

    public void RageIndicator(){
        rageMeter++;
        spriteRenderer.color = new Color(1, ((12 - 1) - rageMeter) / 11f, ((12 - 1) - rageMeter) / 11f);
    }
}

