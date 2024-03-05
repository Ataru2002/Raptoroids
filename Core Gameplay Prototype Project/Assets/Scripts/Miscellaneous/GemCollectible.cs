using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemCollectible : MonoBehaviour
{
    [SerializeField] int gemValue;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            CombatStageManager.Instance.OnGemPickup(gemValue);
            Destroy(gameObject);
        }
    }
}
