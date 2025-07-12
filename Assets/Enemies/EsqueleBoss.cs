using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EsqueleBoss : NavEnemyBase
{
    [Header("Projectile Settings")]
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float shootDelay = 0.5f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private GameObject firePatchPrefab;

    [Header("Attack Settings")]
    [SerializeField] private int orbitalFireballCount = 8;
    [SerializeField] private float spiralShotInterval = 0.1f;
    [SerializeField] private int spiralShotCount = 12;
    [SerializeField] private float lungeSpeed = 10f;
    [SerializeField] private float lungeDistance = 5f;
    [SerializeField] private float lungeCooldown = 3f;

    private Coroutine currentAttackRoutine;
    private float lastLungeTime;
    private int currentPhase = 1;

    protected override void Start()
    {
        base.Start();
        if (projectilePrefab == null)
            projectilePrefab = Resources.Load<GameObject>("Projectiles/EnemyProjectile");
    }

    protected override void Update()
    {
        base.Update();
        CheckPhaseTransition();
    }

    private void CheckPhaseTransition()
    {
        float healthPercent = (float)health / 100f;
        
        if (healthPercent < 0.75f && currentPhase == 1)
        {
            currentPhase = 2;
            StartCoroutine(PhaseTransition());
        }
        else if (healthPercent < 0.5f && currentPhase == 2)
        {
            currentPhase = 3;
            StartCoroutine(PhaseTransition());
        }
    }

    private IEnumerator PhaseTransition()
    {
        // Visual feedback for phase change
        yield return StartCoroutine(FlickerSprite(0.1f, 10, Color.red));
        
        if (currentPhase == 2)
        {
            SummonMinions(2);
        }
        else if (currentPhase == 3)
        {
            SummonMinions(3);
        }
    }

    protected override void Attack()
    {
        if (isInAttackState || Time.time - lastAttackTime < attackCooldown || currentAttackRoutine != null)
            return;

        // Choose attack based on phase and cooldowns
        if (currentPhase == 1)
        {
            currentAttackRoutine = StartCoroutine(OrbitalFireballAttack());
        }
        else if (currentPhase == 2 && Time.time - lastLungeTime > lungeCooldown)
        {
            currentAttackRoutine = StartCoroutine(ChargedLunge());
        }
        else
        {
            currentAttackRoutine = StartCoroutine(SpiralShotAttack());
        }
    }

    private IEnumerator OrbitalFireballAttack()
    {
        isInAttackState = true;
        FacePlayer();
        yield return StartCoroutine(FlickerSprite(0.1f, 6, Color.red));
        animator.SetTrigger("IsAttacking");

        yield return new WaitForSeconds(shootDelay);

        for (int i = 0; i < orbitalFireballCount; i++)
        {
            float angle = i * (360f / orbitalFireballCount);
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;
            
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction.normalized * projectileSpeed;
            }
        }

        lastAttackTime = Time.time;
        isInAttackState = false;
        currentAttackRoutine = null;
    }

    private IEnumerator SpiralShotAttack()
    {
        isInAttackState = true;
        FacePlayer();
        yield return StartCoroutine(FlickerSprite(0.1f, 6, Color.blue));
        animator.SetTrigger("IsAttacking");

        yield return new WaitForSeconds(shootDelay);

        for (int i = 0; i < spiralShotCount; i++)
        {
            float angle = i * (360f / spiralShotCount * 0.5f);
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;
            
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction.normalized * projectileSpeed;
            }
            yield return new WaitForSeconds(spiralShotInterval);
        }

        lastAttackTime = Time.time;
        isInAttackState = false;
        currentAttackRoutine = null;
    }

    private IEnumerator ChargedLunge()
    {
        isInAttackState = true;
        lastLungeTime = Time.time;
        
        // Telegraph the lunge
        yield return StartCoroutine(FlickerSprite(0.1f, 8, Color.red));
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) yield break;
        
        Vector2 lungeDirection = (player.transform.position - transform.position).normalized;
        animator.SetTrigger("IsAttacking");
        
        // Lunge movement
        float distanceMoved = 0f;
        while (distanceMoved < lungeDistance)
        {
            float moveAmount = lungeSpeed * Time.deltaTime;
            transform.Translate(lungeDirection * moveAmount);
            distanceMoved += moveAmount;
            yield return null;
        }

        // Leave fire patches
        Instantiate(firePatchPrefab, transform.position, Quaternion.identity);

        isInAttackState = false;
        currentAttackRoutine = null;
    }

    private void SummonMinions(int count)
    {
        if (minionPrefab == null) return;
        
        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * 2f;
            Instantiate(minionPrefab, spawnPos, Quaternion.identity);
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

    protected override void Die()
    {
        // Show win screen when EsqueleBoss is defeated
        GameManager.Instance.ShowWinScreen();
        // Call base implementation to maintain existing death functionality
        base.Die();
    }
}
