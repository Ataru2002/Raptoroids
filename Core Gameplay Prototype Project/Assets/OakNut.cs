using System.Collections;
using UnityEngine;

public class OakNut : MonoBehaviour
{
    public float fallSpeed = 2f;  // Adjust this to control the speed of the oak nut falling
    public float freezeDuration = 5f;  // Duration in seconds for player freeze

    bool isFrozen = false;

    void Update()
    {
        if (!isFrozen)
        {
            // Move the oak nut downwards
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isFrozen)
        {
            // Freeze the player
            isFrozen = true;
            StartCoroutine(UnfreezePlayerAfterDelay());
        }

        // Destroy the oak nut when it collides with the player
        Destroy(gameObject);
        }

    }

    IEnumerator UnfreezePlayerAfterDelay()
    {
        yield return new WaitForSeconds(freezeDuration);
        isFrozen = false;
    }
}
