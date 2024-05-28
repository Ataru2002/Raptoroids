using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IBulletHittable
{
    [SerializeField] int maxHealth = 0;
    int currentHealth = 0;
    [SerializeField] ParticleSystem impactParticles;
    PlayerHealthUI healthUI;
    [SerializeField] Hill hills;

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

        impactParticles.Emit(10);
        currentHealth -= 1;
        healthUI.UpdateHealth((float)currentHealth / maxHealth);

        if (currentHealth <= 0 && CombatStageManager.Instance != null)
        {
            // Decouple the particle system from this gameobject so that it may continue playing
            impactParticles.transform.parent = null;
            gameObject.SetActive(false);
            CombatStageManager.Instance.OnPlayerDefeated();
        }
        else if(currentHealth <= 0) {
            // Decouple the particle system from this gameobject so that it may continue playing
            impactParticles.transform.parent = null;
            gameObject.SetActive(false);
            TutorialRoomManager.Instance.OnPlayerDefeated();
        }
    }

    public void hillCollide()
    {
        print("Player hit by hill!");
        currentHealth -= 1;
    }
}
