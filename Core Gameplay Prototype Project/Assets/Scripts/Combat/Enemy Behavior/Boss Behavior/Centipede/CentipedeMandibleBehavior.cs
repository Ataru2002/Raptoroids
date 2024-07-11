using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeMandibleBehavior : MonoBehaviour, IBulletHittable
{
    int mandibleHP = 6;
    EnemyHealth centipedeHP;

    SpriteRenderer spriteRenderer;
    int mandibleState = 0;

    const int pointsAwarded = 50;

    [SerializeField] Sprite[] sprites;
    [SerializeField] Material[] materials;

    bool destroyed = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        centipedeHP = GetComponentInParent<EnemyHealth>();
    }

    public void OnBulletHit(int damage = 1)
    {
        if (destroyed)
        {
            return;
        }

        int finalDamage = Mathf.Clamp(mandibleState * damage, 0, mandibleHP);
        mandibleHP -= finalDamage;

        CombatStageManager.Instance.UpdateScore(pointsAwarded * mandibleState * damage);
        centipedeHP.TakeDamage(finalDamage);

        if (mandibleHP <= 0)
        {
            destroyed = true;
            Destroy(gameObject);
        }
    }

    public void SetState(int val)
    {
        mandibleState = val;
        spriteRenderer.sprite = sprites[mandibleState];
        spriteRenderer.material = materials[mandibleState];
    }
}
