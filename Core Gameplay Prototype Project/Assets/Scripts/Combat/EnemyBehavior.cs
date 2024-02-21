using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour, IBulletHittable
{
    [SerializeField] bool trackPlayer;

    Transform playerTransform;

    SpriteRenderer spriteRenderer;

    float timeSinceSpawn = 0;
    [SerializeField] float timeToFinalPosition = 0;
    protected Vector2 initialPosition;
    protected Vector2 finalPosition;
    public bool FinalPositionReached { get { return timeSinceSpawn >= timeToFinalPosition; } }

    // Start is called before the first frame update
    protected void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!FinalPositionReached)
        {
            timeSinceSpawn += Time.deltaTime;
            
            // Ensure the timer doesn't count beyond the specified limit,
            // since this will be used to determine the enemy's current position.
            if (timeSinceSpawn >= timeToFinalPosition)
            {
                timeSinceSpawn = timeToFinalPosition;
            }

            Vector2 currentPosition = Vector2.Lerp(initialPosition, finalPosition, timeSinceSpawn / timeToFinalPosition);
            transform.position = currentPosition;
        }

        if (trackPlayer)
        {
            Vector2 posDelta = (playerTransform.position - transform.position).normalized;
            float angle = Mathf.Atan2(posDelta.y, posDelta.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void SetInitialPosition(Vector2 val)
    {
        initialPosition = val;
    }

    public void SetFinalPosition(Vector2 val)
    {
        finalPosition = val;
    }

    public void OnBulletHit()
    {
        // Do not allow the player to hit the enemy before the enemy comes on screen.
        if (!spriteRenderer.isVisible)
        {
            return;
        }

        print(gameObject.name + " was hit by a bullet!");

        // TODO: implement some form of health system
        if (CombatStageManager.Instance != null)
        {
            gameObject.SetActive(false);
            CombatStageManager.Instance.OnEnemyDefeated();
        }
    }
}
