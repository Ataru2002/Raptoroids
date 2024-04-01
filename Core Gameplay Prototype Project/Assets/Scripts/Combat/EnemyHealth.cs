using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour, IBulletHittable
{
    // Start is called before the first frame update
    SpriteRenderer spriteRenderer;
    [SerializeField] int maxHealth = 5;
    private int currentHealth;
    LootDropper lootDropper;

    // Used to notify boss behavior script of changes in remaining health
    [SerializeField] HealthChangeEvent OnHealthChange;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        lootDropper = GetComponent<LootDropper>();
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

        OnHealthChange.Invoke((float)currentHealth / maxHealth);

        if (CombatStageManager.Instance != null)
        {
            if (CombatStageManager.Instance.isBossStage)
            {
                CombatStageManager.Instance.UpdateBossHealthBar((float)currentHealth / maxHealth);
            }

            if (currentHealth <= 0)
            {
                if (lootDropper != null)
                {
                    lootDropper.DropLoot();
                }

                CombatStageManager.Instance.OnEnemyDefeated();

                Destroy(gameObject);
            }
        }
    }
}

[System.Serializable]
public class HealthChangeEvent : UnityEvent <float>
{

}
