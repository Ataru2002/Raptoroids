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
    [SerializeField] float slowFollowSpeed;
    [SerializeField] float mediumFollowSpeed;
    [SerializeField] float fastFollowSpeed;
    [SerializeField] float yOffset = 0.5f;
    [SerializeField] float snapDistance = 0.01f;

    float selectedFollowSpeed;
    bool isFrozen = false;
    float freezeDuration = 2f;

    void Start()
    {
        SetSpeed(SpeedTier.Medium);
    }

    void Update()
    {
        if (!isFrozen && Input.GetMouseButton(0))
        {
            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target += Vector3.up * yOffset;
            MoveTowards(target);
        }
    }

    public void SetSpeed(string tier)
    {
        SetSpeed((SpeedTier)Enum.Parse(typeof(SpeedTier), tier));
    }

    private void SetSpeed(SpeedTier tier)
    {
        switch(tier)
        {
            case SpeedTier.Slow:
                selectedFollowSpeed = slowFollowSpeed;
                break;
            case SpeedTier.Medium:
                selectedFollowSpeed = mediumFollowSpeed;
                break;
            case SpeedTier.Fast:
                selectedFollowSpeed = fastFollowSpeed;
                break;
            default:
                print("Unrecognized speed tier argument");
                return;
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
            transform.position = Vector2.MoveTowards(transform.position, target, selectedFollowSpeed * Time.deltaTime);
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