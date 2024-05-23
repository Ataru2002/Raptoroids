using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalcoAbility : RaptoroidAbility
{
    Collider2D collision;
    [SerializeField] GameObject afterImageOriginal;

    [SerializeField] float overdriveDuration;
    float overdriveTimer;

    bool inOverdrive = false;
    Coroutine afterImageSpawn;

    void Awake()
    {
        collision = GetComponent<Collider2D>();
        overdriveTimer = overdriveDuration;
    }

    public void ActivateAbility()
    {
        if (!inOverdrive && cooldownTimer <= 0)
        {
            inOverdrive = true;
            collision.enabled = false;
            afterImageSpawn = StartCoroutine(SpawnAfterImage());
        }
    }

    protected override void UpdateAbilityDuration()
    {
        overdriveTimer -= Time.deltaTime;
        if (overdriveTimer <= 0)
        {
            DeactivateAbility();
        }
    }

    protected override void DeactivateAbility()
    {
        inOverdrive = false;
        collision.enabled = true;
        StopCoroutine(afterImageSpawn);
    }

    IEnumerator SpawnAfterImage()
    {
        while (true)
        {
            Instantiate(afterImageOriginal, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.3f);
        }
    }
}
