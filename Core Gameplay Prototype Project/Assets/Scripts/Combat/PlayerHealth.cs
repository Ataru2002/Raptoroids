using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IBulletHittable
{
    [SerializeField] int maxHealth = 0;
    int currentHealth = 0;
    public HealthUI healthUIText;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthUIText = GetComponent<HealthUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBulletHit()
    {
        print("Player hit by an enemy bullet!");

        currentHealth -= 1;
        healthUIText.updateHealth(currentHealth);
        if (currentHealth <= 0 && CombatStageManager.Instance != null)
        {
            gameObject.SetActive(false);
            CombatStageManager.Instance.OnPlayerDefeated();
        }
    }
}
