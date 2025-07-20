using System;
using System.Collections.Generic;
using UnityEngine;

public class BolaDeFuego : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 1000; // Default damage value for the fireball
    private Vector2 direction;
    private ParticleSystem trailEffect;
    public GameObject impactEffectPrefab;
    private SpriteRenderer spriteRenderer;
    private bool isBeingDestroyed = false;

    // Effects system
    private List<FireballEffect> activeEffects = new List<FireballEffect>();
    public event Action<Vector2> OnWallHit;
    public Vector2 GetDirection() => direction;

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
            // Reset rotation and flip
            spriteRenderer.flipX = false;
            transform.rotation = Quaternion.Euler(0, 0, 0);

            // Apply rotation based on direction
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                // Horizontal movement
                spriteRenderer.flipX = direction.x < 0;
            }
            else
            {
                // Vertical movement
                if (direction.y > 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 90); // Up
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 0, -90); // Down
                }
            }
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
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Calculate hit normal for bounce effect
            Vector2 hitNormal = (transform.position - collision.transform.position).normalized;
            OnWallHit?.Invoke(hitNormal);

            // Check if we have bounce effect, if not, destroy as normal
            bool hasBounceEffect = activeEffects.Exists(effect => effect is BounceEffect);
            if (!hasBounceEffect)
            {
                DestroyFireball();
            }
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            var enemy = collision.GetComponent<NavEnemyBase>();
            if (enemy != null)
            {
                enemy.ApplyDamage(damage);
            }
            DestroyFireball();
        }
    }

    public void DisableFireball()
    {
        isBeingDestroyed = false;
        GetComponent<Collider2D>().enabled = true;
        
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
    private void DestroyFireball()
    {
        if (isBeingDestroyed) return;
        isBeingDestroyed = true;

        // Disable collision immediately
        GetComponent<Collider2D>().enabled = false;

        if (trailEffect != null)
            trailEffect.Stop();

        PoolImpactEffect impactPool = FindObjectOfType<PoolImpactEffect>();
        if (impactPool != null)
            impactPool.GetImpactEffect(transform.position);

        ClearAllEffects();
        //Invoke(nameof(DisableFireball), 0.2f);
        DisableFireball();
    }

private void AdjustParticleEffect()
{
    if (trailEffect == null) return;

    Vector2 dir = direction.normalized;
    Vector3 trailOffset = Vector3.zero;

    // Calculate the position behind the sprite based on direction and sprite state
    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
    {
        // Horizontal movement
        if (spriteRenderer != null)
        {
            // Position based on sprite flip state
            trailOffset.x = spriteRenderer.flipX ? 1f : -1f; // Behind the sprite
        }
        else
        {
            // Fallback if no sprite renderer
            trailOffset.x = dir.x > 0 ? -1f : 1f;
        }
        trailOffset.y = 0; // Center vertically
    }
    else
    {
        // Vertical movement
        trailOffset.y = dir.y > 0 ? -1f : 1f; // Behind the sprite
        trailOffset.x = 0; // Center horizontally
    }

    // Apply the offset
    trailEffect.transform.localPosition = trailOffset;

    // Rotate the particle effect to match the sprite's rotation
    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
    {
        // Horizontal - no rotation needed
        trailEffect.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    else
    {
        // Vertical - rotate to match sprite
        if (dir.y > 0)
        {
            trailEffect.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            trailEffect.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
    }
}

    public void AddEffect<T>(float duration = -1f) where T : FireballEffect
    {
        T effect = gameObject.AddComponent<T>();
        effect.Initialize(this, duration);
        effect.ApplyEffect();
        activeEffects.Add(effect);
    }

    public void AddEffect(FireballEffect effectPrefab, float duration = -1f)
    {
        FireballEffect effect = gameObject.AddComponent(effectPrefab.GetType()) as FireballEffect;
        effect.Initialize(this, duration);
        effect.ApplyEffect();
        activeEffects.Add(effect);
    }

    public void RemoveEffect<T>() where T : FireballEffect
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            if (activeEffects[i] is T)
            {
                activeEffects[i].RemoveEffect();
                Destroy(activeEffects[i]);
                activeEffects.RemoveAt(i);
            }
        }
    }

    public void ClearAllEffects()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            activeEffects[i].RemoveEffect();
            Destroy(activeEffects[i]);
        }
        activeEffects.Clear();
    }

}
