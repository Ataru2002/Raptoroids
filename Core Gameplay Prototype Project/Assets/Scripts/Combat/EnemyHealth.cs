using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour, IBulletHittable
{
    // Start is called before the first frame update
    SpriteRenderer spriteRenderer;
    [SerializeField] int maxHealth = 5;
    private int currentHealth;

    [SerializeField] int pointsAwarded;

    // Used to notify boss behavior script of changes in remaining health
    [SerializeField] HealthChangeEvent OnHealthChange;

    public EnemyDefeatEvent OnDefeat;

    // Ensure that multi-projectile attacks don't trigger death more than once
    bool dead = false;
    

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBulletHit(int damage = 1)
    {
        // Do not allow the player to hit the enemy before the enemy comes on screen.
        if (!spriteRenderer.isVisible)
        {
            return;
        }

        currentHealth -= damage;
        NotifyUpdateHealth();
        CombatSFXManager.PlaySoundAtLocation("EnemyHit", transform.position);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        NotifyUpdateHealth();
    }

    void NotifyUpdateHealth()
    {
        OnHealthChange.Invoke((float)currentHealth / maxHealth);

        if (CombatStageManager.Instance != null)
        {
            if (CombatStageManager.Instance.isBossStage)
            {
                CombatStageManager.Instance.UpdateBossHealthBar((float)currentHealth / maxHealth);
            }

            if (!dead && currentHealth <= 0)
            {
                dead = true;
                OnDefeat.Invoke();
                GameManager.Instance.EnemiesSinceLastTreasureRoom += 1;

                CombatStageManager.Instance.UpdateScore(pointsAwarded);
                CombatStageManager.Instance.OnEnemyDefeated();

                Destroy(gameObject);
            }
        }
        else{
            if (!dead && currentHealth <= 0)
            {
                dead = true;
                OnDefeat.Invoke();
                GameManager.Instance.EnemiesSinceLastTreasureRoom += 1;

                Destroy(gameObject);
            }
        }
    }
}

[System.Serializable]
public class HealthChangeEvent : UnityEvent <float>
{

}

[System.Serializable]
public class EnemyDefeatEvent : UnityEvent
{

}
