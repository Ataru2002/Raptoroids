using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CondoroidAbility : RaptoroidAbility
{
    PlayerHealth playerHealth;
    int rageMeter;
    SpriteRenderer spriteRenderer;
    [SerializeField] GameObject prefab;
    int totalDamage = 1;
    const int baseDamage = 1;
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
            spawnBullet.GetComponent<BulletBehavior>().SetDamage(RageConvert(3));
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

    private int RageConvert(int hitsToDamageConversion){
        int additionalDamage = rageMeter / hitsToDamageConversion;
        int totalDamage = baseDamage + additionalDamage;
        return totalDamage; 
    }
}

