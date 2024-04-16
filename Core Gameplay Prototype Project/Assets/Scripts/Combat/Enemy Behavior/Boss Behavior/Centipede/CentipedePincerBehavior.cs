using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedePincerBehavior : MonoBehaviour
{
    CircleCollider2D circleCollider;
    ProjectileSpawner projectileSpawner;

    [SerializeField] GameObject vulnerableSpot;

    SpriteRenderer spriteRenderer;
    [SerializeField] Sprite closedSprite;
    [SerializeField] Sprite openSprite;

    EnemyHealth centipedeHP;
    int armHP = 4;

    bool inAttackSequence = false;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        projectileSpawner = GetComponentInChildren<ProjectileSpawner>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        centipedeHP = GetComponentInParent<EnemyHealth>();
    }

    // Start is called before the first frame update
    void Start()
    {
        projectileSpawner.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
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
        spriteRenderer.sprite = openSprite;
        yield return new WaitForSeconds(0.6f);

        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.4f);
            projectileSpawner.ResetShotTimer();
            projectileSpawner.TryShoot();
        }

        circleCollider.enabled = true;
        vulnerableSpot.SetActive(false);
        spriteRenderer.sprite = closedSprite;

        inAttackSequence = false;
    }

    public void NotifyPincerHit()
    {
        centipedeHP.TakeDamage(1);
        armHP--;
        if (armHP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
