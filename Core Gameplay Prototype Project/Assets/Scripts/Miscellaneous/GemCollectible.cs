using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemCollectible : MonoBehaviour
{
    [SerializeField] int gemValue;
    const float lifetime = 10f;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        CombatStageManager.Instance.OnGemSpawn();
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(RunLifetime());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            CombatStageManager.Instance.OnGemPickup(gemValue);
            Destroy(gameObject);
        }
    }

    IEnumerator RunLifetime()
    {
        float halflife = lifetime / 2;
        const float stepTime = 0.2f;
        yield return new WaitForSeconds(halflife);

        float lifeElapsed = halflife;
        int step = 0;
        while (lifeElapsed < lifetime)
        {
            spriteRenderer.color = step % 2 == 0 ? Color.white : new Color(0.4f, 0.4f, 0.4f, 0.6f);
            yield return new WaitForSeconds(stepTime);
            lifeElapsed += stepTime;
            step++;
        }

        CombatStageManager.Instance.OnGemDespawn();
        Destroy(gameObject);
    }
}
