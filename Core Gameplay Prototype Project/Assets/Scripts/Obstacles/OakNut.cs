using System.Collections;
using UnityEngine;

public class OakNut : MonoBehaviour
{
    public float fallSpeed = 2f;  // speed of the oak nut falling
    [SerializeField] GameObject sfxPlayerPrefab;

    void Update()
    {
        transform.Translate(fallSpeed * Time.deltaTime * Vector3.down);
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

            // Spawn a detached sfx player so that the sound can continue playing after the nut itself is destroyed.
            GameObject sfxPlayer = Instantiate(sfxPlayerPrefab, transform.position, Quaternion.identity);
            AudioSource sfxSource = sfxPlayer.GetComponent<AudioSource>();
            Destroy(sfxPlayer, sfxSource.clip.length);

            // Destroy the oak nut when it collides with the player
            Destroy(gameObject);
        }

    }
}
