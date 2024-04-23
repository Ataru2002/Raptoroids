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

    [SerializeField] EnemyDefeatEvent OnDefeat;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBulletHit()
    {
        // Do not allow the player to hit the enemy before the enemy comes on screen.
        if (!spriteRenderer.isVisible)
        {
            return;
        }

        currentHealth--;
        NotifyUpdateHealth();
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

            if (currentHealth <= 0)
            {
                OnDefeat.Invoke();

                CombatStageManager.Instance.OnEnemyDefeated();
                CombatStageManager.Instance.UpdateScore(pointsAwarded);

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
