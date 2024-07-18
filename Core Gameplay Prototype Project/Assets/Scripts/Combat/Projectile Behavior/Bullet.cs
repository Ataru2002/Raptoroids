using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
    Player,
    Enemy,
}

public class Bullet : MonoBehaviour
{
    public bool ReleasedToPool { get; private set; }
    [SerializeField] protected bool pooled = true;
    [SerializeField] protected int bulletPoolID;
    [SerializeField] protected float speed;
    [SerializeField] protected TargetType targetType;

    protected SpriteRenderer spriteRenderer;

    protected delegate void BulletDespawner(GameObject bullet);
    protected BulletDespawner despawn;

    protected bool isPlayerBullet { get { return targetType == TargetType.Enemy; } }
    protected int damage = 1;

    protected void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    protected void Start()
    {
        if (!pooled)
        {
            despawn = Destroy;
        }

        else
        {
            despawn = isPlayerBullet ? ProjectilePoolManager.Instance.ReturnPlayerProjectile : ProjectilePoolManager.Instance.ReturnEnemyProjectile;
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        // Note: by default, Translate translates the transform relative to itself, so Vector3.up
        // is like 1 unit up the y-axis relative to the transform (after rotation)
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (!spriteRenderer.isVisible)
        {
            despawn(gameObject);
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == targetType.ToString())
        {
            if (isPlayerBullet)
            {
                GameObject particles;
                if (CombatStageManager.Instance != null) 
                {
                    particles = CombatStageManager.Instance.GetEnemyHitParticles();
                }
                else 
                {
                    particles = TutorialRoomManager.Instance.GetEnemyHitParticles();
                }
                particles.transform.position = transform.position;
                particles.GetComponent<ParticleSystem>().Emit(10);
            }

            IBulletHittable bulletHitDetector = collision.GetComponent<IBulletHittable>();
            if (bulletHitDetector != null)
            {
                bulletHitDetector.OnBulletHit(damage);
            }

            despawn(gameObject);
        }
    }

    public int GetBulletPoolID()
    {
        return bulletPoolID;
    }

    public void SetSpeed(float val)
    {
        speed = val;
    }

    public void SetTargetType(TargetType val)
    {
        targetType = val;
    }

    public void SetDamage(int val){
        damage = val;
    }

    public void OnGetFromPool()
    {
        ReleasedToPool = false;
    }

    public void OnReleaseToPool()
    {
        ReleasedToPool = true;
    }
}
