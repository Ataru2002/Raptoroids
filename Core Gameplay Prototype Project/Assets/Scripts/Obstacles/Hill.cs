using System.Collections;
using UnityEngine;

public class Hill : MonoBehaviour
{
    public GameObject hillPrefab;
    public GameObject crackingPrefab;
    private GameObject crackingInstance;

    void Start()
    {
        // Start the coroutine to handle the delayed appearance of the cracking object
        StartCoroutine(HandleHillCracking());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")){
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.OnBulletHit();
            }
            Destroy(gameObject);
        }
    }

    IEnumerator HandleHillCracking()
    {
        print("cracking appear");

        yield return new WaitForSeconds(3.0f);

        // Determine a random position in the bottom half of the screen horizontally
        Vector3 bottomHalfPosition = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0.25f, 0.75f), 0.25f, 10));

        crackingInstance = Instantiate(crackingPrefab, bottomHalfPosition, Quaternion.identity);
        crackingInstance.transform.localScale = new Vector3(crackingInstance.transform.localScale.x / 2, crackingInstance.transform.localScale.y, crackingInstance.transform.localScale.z);

        // Make the cracking object blink 3 times
        for (int i = 0; i < 3; i++)
        {
            crackingInstance.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            crackingInstance.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
        Destroy(crackingInstance);
        GameObject newHill = Instantiate(hillPrefab, crackingInstance.transform.position, Quaternion.identity);
        Destroy(newHill, 3.0f);
    }
}
