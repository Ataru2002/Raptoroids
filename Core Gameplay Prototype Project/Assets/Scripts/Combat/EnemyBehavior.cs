using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] bool trackPlayer;

    Transform playerTransform;
    float timeSinceSpawn = 0;
    [SerializeField] float timeToFinalPosition = 0;
    protected Vector2 initialPosition;
    protected Vector2 finalPosition;
    public bool FinalPositionReached { get { return timeSinceSpawn >= timeToFinalPosition; } }

    // Start is called before the first frame update
    protected void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        
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

    
}
