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
    bool isFrozen = false;
    float freezeDuration = 2f;

    void Start(){
        joystickController = FindFirstObjectByType<JoystickController>();
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
        if(joystickController.joystickVec.y != 0){
            Vector3 direction = new Vector3(joystickController.joystickVec.x, joystickController.joystickVec.y, 0f);
            Vector3 newPos = transform.position + followSpeed * Time.deltaTime * direction;
            transform.position = newPos;
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