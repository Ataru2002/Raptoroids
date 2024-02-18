using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerAbility : MonoBehaviour
{
    public GameObject shieldObject;
    public Button shieldButton;

    TargetType targetType;

    public float cooldown = 10f;
    private bool shieldActive = false;

    private float cooldownTimer = 0f;
    public float shieldDuration = 5f;
    private float shieldTimer = 0f;
    // Start is called before the first frame update
    public bool shieldPermanent = true;
    void Start()
    {
        // shieldButton.onClick.AddListener(activateShield);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Jump")){
            activateShield();
        }
        diminishCooldown();
        updateShieldDuration();
    }

    public void activateShield(){
        print("Ability activated");
        if(!shieldActive && cooldownTimer <= 0){
            shieldActive = true;
            shieldObject.SetActive(true);
            cooldownTimer = cooldown;
            if(!shieldPermanent){
                shieldTimer = shieldDuration;
            }
        }
    }

    void updateShieldDuration(){
        if(shieldActive && !shieldPermanent){
            shieldTimer -= Time.deltaTime;
            if(shieldTimer <= 0f){
                deactivateShield();
            }
        }
    }

    public void deactivateShield(){
        shieldActive = false;
        shieldObject.SetActive(false);
    }

    void diminishCooldown(){
        if(cooldownTimer > 0f){
            shieldButton.interactable = false;
            cooldownTimer -= Time.deltaTime;
            cooldownTimer = Mathf.Max(0f, cooldownTimer);   //make sure that it does not go below 0
        } else{
            shieldButton.interactable = true;
            deactivateShield();
        }

        updateCooldownText();
    }

    void updateCooldownText(){
        shieldButton.GetComponentInChildren<TMP_Text>().text = cooldownTimer.ToString("F1");
    }

    
    
}
