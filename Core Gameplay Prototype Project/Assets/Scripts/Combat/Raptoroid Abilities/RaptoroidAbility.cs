using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class RaptoroidAbility : MonoBehaviour
{
    protected float cooldownTimeRemaining = 0f;
    public float cooldownDuration = 10f;
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

        UpdateCooldownHUD();
    }

    protected virtual void UpdateCooldownHUD()
    {
        float timeElapsed = cooldownDuration - cooldownTimeRemaining;
        cooldownUI.UpdateCooldownProgress(timeElapsed / cooldownDuration);
    }
}
