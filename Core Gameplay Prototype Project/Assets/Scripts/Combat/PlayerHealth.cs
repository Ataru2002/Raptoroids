using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IBulletHittable
{
    [SerializeField] int maxHealth = 0;
    int currentHealth = 0;
    [SerializeField] ParticleSystem impactParticles;
    PlayerHealthUI healthUI;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        impactParticles = GetComponentInChildren<ParticleSystem>();
        healthUI = FindFirstObjectByType<PlayerHealthUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBulletHit()
    {
        print("Player hit by an enemy bullet!");

        impactParticles.Emit(5);
        currentHealth -= 1;
        healthUI.UpdateHealth((float)currentHealth / maxHealth);
        if (currentHealth <= 0 && CombatStageManager.Instance != null)
        {
            gameObject.SetActive(false);
            CombatStageManager.Instance.OnPlayerDefeated();
        }
    }
}
