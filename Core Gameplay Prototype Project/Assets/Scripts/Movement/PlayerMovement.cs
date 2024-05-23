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

    bool isFrozen = false;
    float freezeDuration = 2f;

    void Update()
    {
        if (!isFrozen && Input.GetMouseButton(0))
        {
            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target += Vector3.up * yOffset;
            MoveTowards(target);
        }
    }

    private void MoveTowards(Vector2 target)
    {
        if (Vector2.Distance(transform.position, target) <= snapDistance)
        {
            transform.position = new Vector3(target.x, target.y, transform.position.z);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, target, followSpeed * Time.deltaTime);
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