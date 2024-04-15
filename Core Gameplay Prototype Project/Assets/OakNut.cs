using System.Collections;
using UnityEngine;

public class OakNut : MonoBehaviour
{
    public float fallSpeed = 2f;  // speed of the oak nut falling
    public float spawnInterval = 3f;
    private float timer;
    public GameObject oakNutPrefab; 

    void Update()
    {
        // Move the oak nut downwards
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            // Instantiate a new oak nut and reset the timer
            Instantiate(oakNutPrefab, transform.position, Quaternion.identity);
            timer = 0f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
        // Destroy the oak nut when it collides with the player
        Destroy(gameObject);
        }

    }
}
