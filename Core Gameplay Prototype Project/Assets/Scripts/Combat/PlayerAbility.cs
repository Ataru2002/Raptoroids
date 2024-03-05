using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerAbility : MonoBehaviour
{
    [SerializeField] GameObject shieldObject;
    AbilityCooldownUI cooldownUI;

    TargetType targetType;

    public float cooldown = 10f;
    private bool shieldActive = false;

    private float cooldownTimer = 0f;
    public float shieldDuration = 5f;
    private float shieldTimer = 0f;
    public bool shieldPermanent = true;

    private float doubleClickWindow = 0.2f;

    private float lastClickTime = 0f;


    private void Start()
    {
        cooldownUI = FindFirstObjectByType<AbilityCooldownUI>();
    }

    void Update()
    {   
        //double click implementation
        if(Input.GetMouseButtonDown(0)) {
            float sinceLastClick = Time.time - lastClickTime;

            if(sinceLastClick <= doubleClickWindow){
                activateShield();
            }
            lastClickTime = Time.time;
        }

        diminishCooldown();
        updateShieldDuration();
    }

    public void activateShield() {
        print("Ability activated");
        if(!shieldActive && cooldownTimer <= 0) {
            shieldActive = true;
            shieldObject.SetActive(true);
            cooldownTimer = cooldown;
            if(!shieldPermanent){                   //can remove after playesting
                shieldTimer = shieldDuration;
            }
        }
    }

    void updateShieldDuration() {
        if(shieldActive && !shieldPermanent) {
            shieldTimer -= Time.deltaTime;
            if(shieldTimer <= 0f){
                deactivateShield();
            }
        }
    }

    public void deactivateShield() {
        shieldActive = false;
        shieldObject.SetActive(false);
    }

    void diminishCooldown() {
        if(cooldownTimer > 0f) {
            cooldownTimer -= Time.deltaTime;
            cooldownTimer = Mathf.Max(0f, cooldownTimer);   //make sure that it does not go below 0
        } else{
            deactivateShield();
        }
        updateCooldownHUD();
    }

    void updateCooldownHUD() {
        float timeRemaining = cooldown - cooldownTimer;
        cooldownUI.UpdateCooldownProgress(timeRemaining / cooldown);
    }
}
