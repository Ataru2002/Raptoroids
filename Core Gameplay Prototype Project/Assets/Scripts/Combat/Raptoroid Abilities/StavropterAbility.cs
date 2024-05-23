using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StavropterAbility : RaptoroidAbility
{
    [SerializeField] GameObject defaultGun;
    [SerializeField] GameObject[] sideGunBulletSpawn;

    private bool sideGunActive = false;

    public float sideGunDuration = 4f;

    private float sideGunTimer = 0f;

    public void activateSideGun(){
        print("Side Gun Activated");
        if(!sideGunActive && cooldownTimer <= 0){
            sideGunActive = true;
            defaultGun.SetActive(false);
            foreach(GameObject bullet in sideGunBulletSpawn){
                bullet.SetActive(true);
            }
            cooldownTimer = cooldown;
            sideGunTimer = sideGunDuration;
        }
    } 

    protected override void UpdateAbilityDuration(){
        sideGunTimer -= Time.deltaTime;
        if (sideGunTimer <= 0f)
        {
            DeactivateAbility();
        }
    }

    protected override void DeactivateAbility(){
        sideGunActive = false;
        //sideGun Obj setactive false?
        defaultGun.SetActive(true);
        foreach(GameObject bullet in sideGunBulletSpawn){
                bullet.SetActive(false);
        }
    }
}
