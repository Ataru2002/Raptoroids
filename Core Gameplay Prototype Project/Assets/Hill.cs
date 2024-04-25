using System.Collections;
using UnityEngine;

public class Hill : MonoBehaviour
{
    public float fallSpeed = 3f;  // speed of the hill falling
    private float timer;
    public GameObject hillPrefab;

    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")){
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            health.hillCollide();
            Destroy(gameObject);
        }
    }
}
