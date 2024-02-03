using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour, IBulletHittable
{
    [SerializeField] bool trackPlayer;

    Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (trackPlayer)
        {
            Vector2 posDelta = (playerTransform.position - transform.position).normalized;
            float angle = Mathf.Atan2(posDelta.y, posDelta.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void OnBulletHit()
    {
        print(gameObject.name + " was hit by a bullet!");
    }
}
