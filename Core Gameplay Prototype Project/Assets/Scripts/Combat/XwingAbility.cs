using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XwingAbility : MonoBehaviour
{
    [SerializeField] GameObject defaultGun;
    [SerializeField] GameObject[] sideGunBulletSpawn;

    AbilityCooldownUI cooldownUI;
    public float cooldown = 10f;

    private bool sideGunActive = false;

    private float cooldownTimer = 0f;

    public float sideGunDuration = 4f;

    private float sideGunTimer = 0f;

    public bool sideGunPermanent = false;

    private float doubleClickWindow = 0.2f;
    private float lastClickTime = 0f;
    void Start()
    {
        cooldownUI = FindFirstObjectByType<AbilityCooldownUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) {
            float sinceLastClick = Time.time - lastClickTime;

            if(sinceLastClick <= doubleClickWindow){
                activateSideGun();
            }
            lastClickTime = Time.time;
        }

        diminishCDSideGun();
        updateSideGunDuration();
    }

    public void activateSideGun(){
        print("Side Gun Activated");
        if(!sideGunActive && cooldownTimer <= 0){
            sideGunActive = true;
            defaultGun.SetActive(false);
            foreach(GameObject bullet in sideGunBulletSpawn){
                bullet.SetActive(true);
            }
            cooldownTimer = cooldown;
            if(!sideGunPermanent){
                sideGunTimer = sideGunDuration;
            }
        }
    } 

    public void updateSideGunDuration(){
        if(sideGunActive && !sideGunPermanent){
            sideGunTimer -= Time.deltaTime;
            if(sideGunTimer <= 0f){
                deactivateSideGun();
            }
        }
    }

    void deactivateSideGun(){
        sideGunActive = false;
        //sideGun Obj setactive false?
        defaultGun.SetActive(true);
        foreach(GameObject bullet in sideGunBulletSpawn){
                bullet.SetActive(false);
        }

    }

    void diminishCDSideGun(){
        if(cooldownTimer > 0f){
            cooldownTimer -= Time.deltaTime;
            cooldownTimer = Mathf.Max(0f, cooldownTimer);
        } else{
            deactivateSideGun();
        }
        updateCDSideGunHUD();
    }

    void updateCDSideGunHUD(){
        float timeRemaining = cooldown - cooldownTimer;
        cooldownUI.UpdateCooldownProgress(timeRemaining / cooldown);
    }    
}
