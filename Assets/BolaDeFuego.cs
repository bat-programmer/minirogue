using UnityEngine;

public class BolaDeFuego : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 direction;
    private ParticleSystem trailEffect;
    public GameObject impactEffectPrefab;  // Add this
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
        // Flip sprite if moving left
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction.x < 0;
        }
        AdjustParticleEffect();
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
                // Tell the enemy it got hit (optional but cleaner)
                var enemy = collision.GetComponent<EsqueletoEspada>();
                if (enemy != null)
                {                    
                    enemy.ApplyDamage(10); // New method you define
                }
            }

            if (trailEffect != null)
            {
                trailEffect.Stop();
            }

            if (impactEffectPrefab != null)
            {
                Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
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
            var main = trailEffect.main;

            // Reverse the particle system if moving left
            trailEffect.transform.localRotation = direction.x < 0
                ? Quaternion.Euler(0, 180, 0)
                : Quaternion.identity;
        }
    }
}
