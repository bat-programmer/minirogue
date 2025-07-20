using UnityEngine;
using System.Collections;

public class VampireEnemy : NavEnemyBase
{
    [Header("Vampire Settings")]
    public float lungeSpeed = 8f;
    public float lungeDistance = 2f;
    public float windupDuration = 0.5f;
    public float recoveryDuration = 0.3f;

    private bool isLunging = false;
    private Vector2 lungeDirection;

    protected override void Start()
    {
        base.Start();
        
        // Setup animation event forwarding
        var forwarder = GetComponentInChildren<AnimationEventForwarder>();
        if (forwarder != null)
        {
            forwarder.parentEnemy = this;
        }
    }

public override void ApplyDamage(int damage)
{
    Debug.Log($"[VampireEnemy] ApplyDamage called. isDead={isDead}, isInDamageState={isInDamageState}, health={health}");

    if (isDead) return;

    if (!isInDamageState)
    {
        Debug.Log("[VampireEnemy] Taking damage. Interrupting lunge/attack.");
        isLunging = false;
        isInAttackState = false;
        animator.ResetTrigger("IsAttacking");
        StopAllCoroutines();
        base.ApplyDamage(damage);
    }
    else
    {
        Debug.Log("[VampireEnemy] Ignored damage due to damage state.");
    }
}

    // Animation event handlers
    public void OnWindupStart()
    {
        FacePlayer();
        StartCoroutine(WindupCoroutine());
    }

    public void OnLungeStart()
    {
        isLunging = true;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            lungeDirection = (player.transform.position - transform.position).normalized;
        }
    }

    public void OnLungeEnd()
    {
        isLunging = false;
    }

    private IEnumerator WindupCoroutine()
    {
        yield return new WaitForSeconds(windupDuration);
        animator.SetTrigger("IsAttacking");
    }

    protected override void Update()
    {
        base.Update();

        if (isLunging)
        {
            transform.Translate(lungeDirection * lungeSpeed * Time.deltaTime);
        }
    }

    protected override void Attack()
    {
        if (isInAttackState || Time.time - lastAttackTime < attackCooldown)
            return;

        animator.SetTrigger("IsAttacking");
        lastAttackTime = Time.time;
    }

    // Called during lunge frames to deal damage
    public void DealLungeDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.ApplyDamage(1);
                }
            }
        }
    }
}
