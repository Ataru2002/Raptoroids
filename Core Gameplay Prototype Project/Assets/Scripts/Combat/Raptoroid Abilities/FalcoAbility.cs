using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalcoAbility : RaptoroidAbility
{
    Collider2D collision;
    SpriteRenderer sprite;

    [SerializeField] GameObject afterImageOriginal;

    [SerializeField] float overdriveDuration;
    float overdriveTimer;

    bool inOverdrive = false;
    Coroutine afterImageSpawn;

    void Awake()
    {
        collision = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        overdriveTimer = overdriveDuration;
    }

    public void ActivateAbility()
    {
        if (!inOverdrive && cooldownTimeRemaining <= 0)
        {
            cooldownTimeRemaining = cooldownDuration;

            inOverdrive = true;
            collision.enabled = false;
            overdriveTimer = overdriveDuration;
            afterImageSpawn = StartCoroutine(SpawnAfterImage());
            sprite.color = Color.red;
            BroadcastMessage("FireRateModify", 3f, SendMessageOptions.DontRequireReceiver);
        }
    }

    protected override void UpdateAbilityDuration()
    {
        if (inOverdrive)
        {
            overdriveTimer -= Time.deltaTime;
            if (overdriveTimer <= 0)
            {
                DeactivateAbility();
            }
        }
    }

    protected override void DeactivateAbility()
    {
        inOverdrive = false;
        collision.enabled = true;
        StopCoroutine(afterImageSpawn);
        sprite.color = Color.white;
        BroadcastMessage("ResetFireRate", SendMessageOptions.DontRequireReceiver);
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
