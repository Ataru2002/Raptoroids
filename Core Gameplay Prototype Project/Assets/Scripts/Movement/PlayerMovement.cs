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
    private Vector3 previousPos;
    bool isFrozen = false;
    float freezeDuration = 2f;
    float joystickSpeedMult = 0.5f;
    void Start(){
        joystickController = FindFirstObjectByType<JoystickController>();
        previousPos = transform.position;
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

        float distanceTraveled = Vector3.Distance(previousPos, transform.position);
    }

    private void JoystickMoveTowards(){
        if(Math.Abs(joystickController.joystickVec.magnitude) != 0){
            Vector3 direction = new Vector3(joystickController.joystickVec.x, joystickController.joystickVec.y, 0f);
            direction = direction.magnitude > 1f ? direction.normalized : direction;
            Vector3 newPos = direction * followSpeed * Time.deltaTime * joystickSpeedMult;


            float distanceTraveled = Vector3.Distance(previousPos, newPos);    
            transform.position += newPos;

            previousPos = newPos;
        }
    }

    public void FreezePlayer()
    {
        isFrozen = true;
        StartCoroutine(UnfreezePlayerAfterDelay());
    }

    IEnumerator UnfreezePlayerAfterDelay()
    {
        yield return new WaitForSeconds(freezeDuration);
        isFrozen = false;
    }
}