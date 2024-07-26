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
    [SerializeField] SpriteRenderer shieldRenderer;
    
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
        if(!shieldActive && cooldownTimeRemaining <= 0) {
            shieldActive = true;
            shieldObject.SetActive(true);

            // Set the shader's start time to the current shader time
            // to make the ripple start from the center
            float currentShaderTime = Shader.GetGlobalVector("_Time").y;
            shieldRenderer.material.SetFloat("_StartTime", currentShaderTime);
            
            cooldownTimeRemaining = cooldownDuration;
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

    protected override void UpdateCooldownVisuals()
    {
        base.UpdateCooldownVisuals();
        if (cooldownTimeRemaining <= 0 && onShieldCDFull != null)
        {
            onShieldCDFull.Invoke();
        }
    }
}
