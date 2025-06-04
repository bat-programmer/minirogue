using System.Collections;
using UnityEngine;

public class EsqueleMago : EnemyBase
{
    private GameObject projectilePrefab;

    [Header("Projectile Settings")]
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float shootDelay = 0.5f;

    private Coroutine currentAttackRoutine;

    protected override void Start()
    {
        base.Start();

        projectilePrefab = Resources.Load<GameObject>("Projectiles/EnemyProjectile");

        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab not found in Resources/Projectiles/EnemyProjectile");
        }
    }

    protected override void Attack()
    {
        if (isInAttackState || Time.time - lastAttackTime < attackCooldown || currentAttackRoutine != null)
            return;

        currentAttackRoutine = StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        isInAttackState = true;

        FacePlayer();
        yield return StartCoroutine(FlickerSprite(0.1f, 6, Color.red));

        animator.SetTrigger("IsAttacking");

        yield return new WaitForSeconds(shootDelay); // allow animation wind-up

        ShootProjectileAtPlayer();

        lastAttackTime = Time.time;
        isInAttackState = false;
        currentAttackRoutine = null;
    }

    private void ShootProjectileAtPlayer()
    {
        if (projectilePrefab == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector2 direction = (player.transform.position - transform.position).normalized;

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }
    }

    private IEnumerator FlickerSprite(float flickerInterval, int flickerCount, Color flickerColor)
    {
        Color originalColor = Color.white;

        for (int i = 0; i < flickerCount; i++)
        {
            spriteRenderer.color = (i % 2 == 0) ? flickerColor : originalColor;
            yield return new WaitForSeconds(flickerInterval);
        }
        spriteRenderer.color = originalColor;
    }
}
