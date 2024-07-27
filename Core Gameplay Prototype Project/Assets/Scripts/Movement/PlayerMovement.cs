using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpeedTier
{
    Slow,
    Medium,
    Fast
}

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float followSpeed;
    [SerializeField] float yOffset = 0.5f;
    [SerializeField] float snapDistance = 0.01f;
    JoystickController joystickController;

    StatusEffectHandler statusHandler;
    bool isFrozen { 
        get
        {
            return statusHandler.HasStatusCondition(StatusEffect.Stun);
        } 
    }
    const float freezeDuration = 2f;
    
    float joystickSpeedMult = 0.5f;

    Vector2 upperBound;
    Vector2 lowerBound;

    void Awake()
    {
        statusHandler = GetComponent<StatusEffectHandler>();
        joystickController = FindFirstObjectByType<JoystickController>();
    }

    void Start() {
        upperBound = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        lowerBound = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
    }

    void Update()
    {
        if (!isFrozen && Input.GetMouseButton(0))
        {
            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target += Vector3.up * yOffset;
            
            if(PlayerPrefs.GetInt("joystick") == 1){
                JoystickMoveTowards();
            }
            else{
                MoveTowards(target);
            }
        }
    }

    private void MoveTowards(Vector2 target)
    {
        if (Time.timeScale > 0 && Vector2.Distance(transform.position, target) <= snapDistance)
        {
            transform.position = new Vector3(target.x, target.y, transform.position.z);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, target, followSpeed * Time.deltaTime);
        }
    }

    private void JoystickMoveTowards(){
        if(Math.Abs(joystickController.joystickVec.magnitude) != 0){
            Vector3 direction = new Vector3(joystickController.joystickVec.x, joystickController.joystickVec.y, 0f);
            direction = direction.magnitude > 1f ? direction.normalized : direction;
            Vector3 newPos = direction * followSpeed * Time.deltaTime * joystickSpeedMult;

            float newX = transform.position.x + newPos.x;
            newX = Mathf.Clamp(newX, lowerBound.x, upperBound.x);
            
            float newY = transform.position.y + newPos.y;
            newY = Mathf.Clamp(newY, lowerBound.y, upperBound.y);

            transform.position = new Vector3(newX, newY, transform.position.z);
        }
    }

    public void FreezePlayer()
    {
        StartCoroutine(UnfreezePlayerAfterDelay());
    }

    IEnumerator UnfreezePlayerAfterDelay()
    {
        statusHandler.SetStatusCondition(StatusEffect.Stun, freezeDuration);
        CombatStageManager.Instance.ToggleOaknutScreen(true, freezeDuration);

        yield return new WaitForSeconds(freezeDuration);
        
        CombatStageManager.Instance.ToggleOaknutScreen(false);
    }
}