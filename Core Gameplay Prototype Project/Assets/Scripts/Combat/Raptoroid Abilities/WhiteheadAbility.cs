using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.NetworkInformation;
using System;
public class WhiteheadAbility : RaptoroidAbility
{
    [SerializeField] GameObject shieldObject;
    
    TargetType targetType;

    private bool shieldActive = false;

    
    public float shieldDuration = 3f;
    private float shieldTimer = 0f;
    public bool shieldPermanent = true;

    public bool isPlayer;

    public static event Action onShieldActivated; 

    public static event Action onShieldCDFull;

    new void Start()
    {
        base.Start();
        if(!isPlayer){
            activateShield();
        }
    }

    new void Update()
    {
        if (isPlayer){
            base.Update();
        }
    }

    public void activateShield() {
        print("Ability activated");
        if(!shieldActive && cooldownTimer <= 0) {
            shieldActive = true;
            shieldObject.SetActive(true);
            cooldownTimer = cooldown;
            onShieldActivated?.Invoke();
            shieldTimer = shieldDuration;
        }
    }

    protected override void UpdateAbilityDuration() {
        if(shieldActive && !shieldPermanent) {
            shieldTimer -= Time.deltaTime;
            if(shieldTimer <= 0f){
                DeactivateAbility();
            }
        }
    }

    protected override void DeactivateAbility() {
        shieldActive = false;
        shieldObject.SetActive(false);
    }

    protected override void UpdateCooldownHUD()
    {
        base.UpdateCooldownHUD();
        print(cooldownTimer);
        if (cooldownTimer <= 0 && onShieldCDFull != null)
        {
            print("On Shield CD Full has listeners");
            onShieldCDFull.Invoke();
        }
    }
}
