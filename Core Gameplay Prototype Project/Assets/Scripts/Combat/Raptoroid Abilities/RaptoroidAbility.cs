using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class RaptoroidAbility : MonoBehaviour
{
    protected float cooldownTimer = 0f;
    public float cooldown = 10f;
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
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownTimer = Mathf.Max(0f, cooldownTimer);   //make sure that it does not go below 0
        }

        UpdateCooldownHUD();
    }

    protected virtual void UpdateCooldownHUD()
    {
        float timeRemaining = cooldown - cooldownTimer;
        cooldownUI.UpdateCooldownProgress(timeRemaining / cooldown);
    }
}
