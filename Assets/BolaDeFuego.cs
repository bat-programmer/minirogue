using UnityEngine;

public class BolaDeFuego : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 1000; // Default damage value for the fireball
    private Vector2 direction;
    private ParticleSystem trailEffect;
    public GameObject impactEffectPrefab;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        trailEffect = GetComponentInChildren<ParticleSystem>();

        if (trailEffect != null)
        {
            trailEffect.Play();
        }
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction.x < 0;
        }
        AdjustParticleEffect();
    }

    // Add this method to set damage from outside
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                var enemy = collision.GetComponent<NavEnemyBase>();
                if (enemy != null)
                {
                    enemy.ApplyDamage(damage);
                }
            }

            if (trailEffect != null)
            {
                trailEffect.Stop();
            }

            PoolImpactEffect impactPool = FindObjectOfType<PoolImpactEffect>();
            if (impactPool != null)
            {
                impactPool.GetImpactEffect(transform.position);
            }

            Invoke(nameof(DisableFireball), 0.2f);
        }
    }

    private void DisableFireball()
    {
        PoolBolaDeFuego pool = FindObjectOfType<PoolBolaDeFuego>();
        if (pool != null)
        {
            pool.ReturnFireball(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void AdjustParticleEffect()
    {
        if (trailEffect != null)
        {
            trailEffect.transform.localRotation = direction.x < 0
                ? Quaternion.Euler(0, 180, 0)
                : Quaternion.identity;
        }
    }
}
