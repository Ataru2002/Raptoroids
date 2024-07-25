using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CondoroidAbility : RaptoroidAbility
{
    [SerializeField] GameObject projectilePrefab;
    SpriteRenderer spriteRenderer;
    int rageMeter;
    const int baseDamage = 1;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rageMeter = 0;
        cooldownDuration = 15;
    }

    public void ActivateAbility(){
        if(rageMeter > 0 && cooldownTimeRemaining <= 0) {
            Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 1, 0);
            GameObject bulletObject = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

            PunchthroughBullet bulletComponent = bulletObject.GetComponent<PunchthroughBullet>();
            bulletComponent.SetDamage(RageConvert(3));

            cooldownTimeRemaining = cooldownDuration;
        }
    }

    // Intentionally left blank
    protected override void UpdateAbilityDuration(){
        
        
    }

    protected override void DeactivateAbility(){

    }

    public void RageIndicator(){
        rageMeter++;

        // TODO: maybe add a shader to make it "glow" ?
        spriteRenderer.color = new Color(1, ((12 - 1) - rageMeter) / 11f, ((12 - 1) - rageMeter) / 11f);
    }

    private int RageConvert(int hitsToDamageConversion){
        int additionalDamage = rageMeter / hitsToDamageConversion;
        int totalDamage = baseDamage + additionalDamage;
        return totalDamage; 
    }
}

