using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IBulletHittable
{
    [SerializeField] int maxHealth = 0;
    int currentHealth = 0;
    [SerializeField] ParticleSystem impactParticles;
    PlayerHealthUI healthUI;

    [SerializeField] UnityEvent onHit;
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

    public void OnBulletHit(int damage = 1)
    {
        print("Player hit by an enemy bullet!");

        impactParticles.Emit(10);

        if (CombatStageManager.Instance != null && CombatStageManager.Instance.stageEnded)
        {
            return;
        }

        currentHealth -= damage;
        onHit.Invoke();
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

        CombatSFXManager.PlaySoundAtLocation("RaptoroidHit", transform.position);
    }
}
