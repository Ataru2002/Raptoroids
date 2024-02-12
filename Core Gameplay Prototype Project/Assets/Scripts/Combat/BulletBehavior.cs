using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
    Player,
    Enemy
}

public class BulletBehavior : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float lifespan = 5.0f;
    [SerializeField] TargetType targetType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Note: by default, Translate translates the transform relative to itself, so Vector3.up
        // is like 1 unit up the y-axis relative to the transform (after rotation)
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        Destroy(gameObject, lifespan);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == targetType.ToString())
        {
            collision.GetComponent<IBulletHittable>().OnBulletHit();
            Destroy(gameObject);
        }
    }
}
