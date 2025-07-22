using UnityEngine;

public class FirePatchEffect : FireballEffect
{
    [SerializeField] private float damageInterval = 0.5f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float duration = 3f;
    private float damageTimer;

    public override void ApplyEffect()
    {
        // Initialize collider
        var collider = gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(1.5f, 1.5f);
        
        // Set default duration if not specified
        if (duration <= 0) duration = 3f;
    }

    public override void UpdateEffect()
    {
        damageTimer += Time.deltaTime;
        if (damageTimer >= damageInterval)
        {
            damageTimer = 0;
            DamagePlayersInRange();
        }
    }

    private void DamagePlayersInRange()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, 
            GetComponent<BoxCollider2D>().size, 0);
        
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.ApplyDamage(damage);
            }
            }
        }
    }

    public override void RemoveEffect()
    {
        isActive = false;
        Destroy(gameObject);
    }
}
