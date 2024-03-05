using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IBulletHittable
{
    // Start is called before the first frame update
    SpriteRenderer spriteRenderer;
    [SerializeField] int maxHealth = 5;
    private int currentHealth;

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

        print(gameObject.name + " was hit by a bullet!");

        currentHealth--;

        if (CombatStageManager.Instance != null)
        {
            if (CombatStageManager.Instance.isBossStage)
            {
                CombatStageManager.Instance.UpdateBossHealthBar((float)currentHealth / maxHealth);
            }

            if (currentHealth <= 0)
            {
                CombatStageManager.Instance.OnEnemyDefeated();
                Destroy(gameObject);
            }
        }
    }
}
