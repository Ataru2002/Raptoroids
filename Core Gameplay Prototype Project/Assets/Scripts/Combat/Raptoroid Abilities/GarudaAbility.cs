using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarudaAbility : RaptoroidAbility
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projectileSpawnPoint;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ActivateAbility()
    {
        Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        cooldownTimeRemaining = cooldownDuration;
    }

    // Intentionally left blank
    protected override void UpdateAbilityDuration()
    {

    }

    protected override void DeactivateAbility()
    {
        
    }

    protected override void UpdateCooldownVisuals()
    {
        base.UpdateCooldownVisuals();
        spriteRenderer.material.SetFloat("_EnergyRatio", rechargeRatio);
    }
}
