using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public float fallSpeed = 2f;
    public GameObject minePrefab;
    public bool trigger = false;
    public GameObject explosionPrefab;
    private GameObject explosionInstance;

    void Start()
    {
        // Start the coroutine to handle the delayed appearance of the cracking object
        transform.localScale = new Vector3(transform.localScale.x / 4, transform.localScale.y / 4, transform.localScale.z);
        trigger = false;
        //StartCoroutine(HandleMineExplosion());
    }

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player")){
    //         PlayerHealth health = other.GetComponent<PlayerHealth>();
    //         health.OnBulletHit();
    //         Destroy(gameObject);
    //     }
    // }

    void Update()
    {
        Vector3 centerPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10));
        if (transform.position.y >= centerPos.y) {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        } else {
            if (!trigger) {
                trigger = true;
                StartCoroutine(HandleMineExplosion());
            }
        }
        
    }

    IEnumerator HandleMineExplosion()
    {
        print("explosion appear");

        yield return new WaitForSeconds(2.0f);
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }

        // Determine a random position in the bottom half of the screen horizontally
        Vector3 centerPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10));

        explosionInstance = Instantiate(explosionPrefab, centerPos, Quaternion.identity);
        explosionInstance.transform.localScale = new Vector3(explosionInstance.transform.localScale.x / 2, explosionInstance.transform.localScale.y / 2, explosionInstance.transform.localScale.z);

        // // Make the cracking object blink 3 times
        // for (int i = 0; i < 3; i++)
        // {
        //     crackingInstance.SetActive(true);
        //     yield return new WaitForSeconds(0.5f);
        //     crackingInstance.SetActive(false);
        //     yield return new WaitForSeconds(0.5f);
        // }
        yield return new WaitForSeconds(0.5f);
        Destroy(explosionInstance);
    }
}
