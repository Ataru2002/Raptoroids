using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public float fallSpeed = 2f;
    public GameObject minePrefab;
    public bool trigger = false;
    public GameObject explosionPrefab;
    public GameObject bulletPrefab;
    private GameObject explosionInstance;
    private GameObject bulletInstance;

    void Start()
    {
        transform.localScale = new Vector3(transform.localScale.x / 4, transform.localScale.y / 4, transform.localScale.z);
        trigger = false;
    }

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

        for (int i = 0; i < 4; i++)
        {
            ShootBullet();
        }

        yield return new WaitForSeconds(0.5f);
        Destroy(explosionInstance);
    }

    void ShootBullet()
    {
        // Instantiate the bullet at the current position
        GameObject bulletInstance = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // Set the bullet's velocity
        float randomAngle = Random.Range(0f, 360f);
        bulletInstance.transform.rotation = Quaternion.Euler(0, 0, randomAngle);

        // Destroy the bullet after a specified lifetime
        //Destroy(bulletInstance, 10.0f);
    }
}
