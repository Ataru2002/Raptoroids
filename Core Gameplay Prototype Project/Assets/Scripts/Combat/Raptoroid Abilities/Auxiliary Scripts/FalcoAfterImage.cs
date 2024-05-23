using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalcoAfterImage : MonoBehaviour
{
    const float maxLifetime = 1.2f;
    float lifetime = maxLifetime;
    SpriteRenderer sprite;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        lifetime -= Time.deltaTime;
        sprite.color = new Color(1, 1, 1, lifetime);

        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
