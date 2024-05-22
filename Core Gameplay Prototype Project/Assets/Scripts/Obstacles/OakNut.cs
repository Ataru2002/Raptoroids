using System.Collections;
using UnityEngine;

public class OakNut : MonoBehaviour
{
    public float fallSpeed = 2f;  // speed of the oak nut falling
    private float timer;
    public GameObject oakNutPrefab;

    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement moveComponent = other.GetComponent<PlayerMovement>();
            if (moveComponent != null)
            {
                moveComponent.FreezePlayer();
            }

            // Destroy the oak nut when it collides with the player
            Destroy(gameObject);
        }

    }
}
