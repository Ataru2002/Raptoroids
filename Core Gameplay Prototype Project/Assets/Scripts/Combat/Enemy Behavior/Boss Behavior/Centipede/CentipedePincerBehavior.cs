using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedePincerBehavior : MonoBehaviour
{
    CircleCollider2D circleCollider;
    ProjectileSpawner projectileSpawner;

    [SerializeField] GameObject vulnerableSpot;

    [SerializeField] SpriteRenderer clawRenderer;

    [SerializeField] Sprite closedSprite;
    [SerializeField] Material closedMaterial;

    [SerializeField] Sprite openSprite;
    [SerializeField] Material openMaterial;

    EnemyHealth centipedeHP;
    const int hpScale = 6;
    int armHP = 4;

    bool inAttackSequence = false;

    bool destroyed = false;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        projectileSpawner = GetComponentInChildren<ProjectileSpawner>();
        centipedeHP = GetComponentInParent<EnemyHealth>();
    }

    // Start is called before the first frame update
    void Start()
    {
        projectileSpawner.enabled = true;
    }

    public void PlayAttackSequence()
    {
        if (!inAttackSequence)
        {
            StartCoroutine(AttackSequence());
        }
    }

    IEnumerator AttackSequence()
    {
        inAttackSequence = true;

        circleCollider.enabled = false;
        vulnerableSpot.SetActive(true);
        clawRenderer.sprite = openSprite;
        clawRenderer.material = openMaterial;
        yield return new WaitForSeconds(0.6f);

        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.4f);
            projectileSpawner.ResetShotTimer();
            projectileSpawner.TryShoot();
        }

        circleCollider.enabled = true;
        vulnerableSpot.SetActive(false);
        clawRenderer.sprite = closedSprite;
        clawRenderer.material = closedMaterial;

        inAttackSequence = false;
    }

    public void NotifyPincerHit(int damage)
    {
        if (destroyed)
        {
            return;
        }

        CombatStageManager.Instance.UpdateScore(15);

        centipedeHP.TakeDamage(hpScale * damage);
        armHP -= damage;

        if (armHP <= 0)
        {
            destroyed = true;
            Destroy(gameObject);
        }
    }
}
