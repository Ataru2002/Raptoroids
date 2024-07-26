using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class RaptoroidAbility : MonoBehaviour
{
    public float cooldownDuration = 10f;
    protected float cooldownTimeRemaining = 0f;
    // Allow subclasses to access timeElapsed and rechargeRatio in case the corresponding Raptoroid
    // does something with ability cooldown
    protected float timeElapsed;
    protected float rechargeRatio;

    protected AbilityCooldownUI cooldownUI;

    // Start is called before the first frame update
    protected void Start()
    {
        cooldownUI = FindFirstObjectByType<AbilityCooldownUI>();
    }

    // Update is called once per frame
    protected void Update()
    {
        DiminishCooldown();
        UpdateAbilityDuration();
    }

    protected virtual void UpdateAbilityDuration()
    {
        throw new NotImplementedException();
    }

    protected virtual void DeactivateAbility()
    {
        throw new NotImplementedException();
    }

    void DiminishCooldown()
    {
        if (cooldownTimeRemaining > 0f)
        {
            cooldownTimeRemaining -= Time.deltaTime;
            cooldownTimeRemaining = Mathf.Max(0f, cooldownTimeRemaining);   //make sure that it does not go below 0
        }

        UpdateCooldownVisuals();
    }

    protected virtual void UpdateCooldownVisuals()
    {
        timeElapsed = cooldownDuration - cooldownTimeRemaining;
        rechargeRatio = timeElapsed / cooldownDuration;
        
        cooldownUI.UpdateCooldownProgress(rechargeRatio);
    }
}
