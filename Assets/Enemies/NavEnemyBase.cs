using UnityEngine;
using UnityEngine.AI;

public abstract class NavEnemyBase : MonoBehaviour
{
    [Header("Movement Type")]
    public bool useNavMesh = true;

    [Header("Enemy Settings")]
    public float speed = 3.0f;
    public int health = 100;
    public float attackRange = 1.5f;
    public float chaseRange = 8.0f;
    public float pathUpdateInterval = 0.5f;

    // States
    protected bool isDead = false;
    protected bool isInAttackState = false;
    protected bool isInDamageState = false;
    protected bool isChasing = false;
    public static event System.Action OnAnyEnemyDied;

    // Components
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected Collider2D enemyCollider;

    protected Vector2[] directions = new Vector2[]
    {
        Vector2.up, Vector2.down, Vector2.left, Vector2.right
    };
    protected Vector2 currentDirection = Vector2.zero;
    protected float pathUpdateTimer = 0f;
    protected float wanderTimer = 0f;
    protected float wanderInterval = 3.0f;
    protected float attackCooldown = 1.0f;
    protected float lastAttackTime = -Mathf.Infinity;
    protected NavMeshAgent navAgent;
    protected virtual void Start()
    {        
        currentDirection = GetRandomCardinalDirection();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();

        if (useNavMesh)
        {
            navAgent = GetComponentInChildren<NavMeshAgent>();
            if (navAgent != null)
            {
                navAgent.updateRotation = false;
                navAgent.updateUpAxis = false;
                navAgent.updatePosition = false;
            }
        }
    }

    protected virtual void Update()
    {
        if (isDead) return;

        if (!isInAttackState && !isInDamageState)
        {
            if (IsPlayerInAttackRange())
                Attack();
            else if (IsPlayerInChaseRange())
                ChasePlayer();
            else
                Wander();
        }
    }

    #region Behavior Methods

    protected virtual void Wander()
    {
        isChasing = false;

        // Change direction occasionally
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= wanderInterval)
        {
            currentDirection = GetRandomCardinalDirection();
            wanderTimer = 0.0f;
        }

        MoveInDirection(currentDirection);

    }
    protected virtual void ChasePlayer()
    {
        isChasing = true;

        if (useNavMesh && navAgent != null)
        {
            // NavMesh chasing
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                navAgent.SetDestination(player.transform.position);
                animator.SetBool("isMoving", true);
                FacePlayer();
            }
        }
        else
        {
            // Direct movement chasing
            pathUpdateTimer += Time.deltaTime;
            if (pathUpdateTimer >= pathUpdateInterval)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Vector2 directionToPlayer = player.transform.position - transform.position;
                    currentDirection = GetBestCardinalDirection(directionToPlayer);
                }
                pathUpdateTimer = 0f;
            }
            MoveInDirection(currentDirection);
        }
    }
    protected virtual void MoveInDirection(Vector2 direction)
    {

        // Check if we can move in this direction
        if (!IsPathBlocked(direction))
        {
            // Move character
            transform.Translate(direction * speed * Time.deltaTime);

            // Update animation
            animator.SetBool("isMoving", true);

            // Flip sprite based on direction
            if (direction.x > 0)
            {
                spriteRenderer.flipX = false; // Facing right
            }
            else if (direction.x < 0)
            {
                spriteRenderer.flipX = true; // Facing left
            }
        }
        else
        {
            // Path is blocked, try a different direction if chasing
            if (isChasing)
            {
                // Try to find a direction that's not blocked
                TryAlternateDirections();
            }
            else
            {
                // If wandering, just pick a new random direction
                currentDirection = GetRandomCardinalDirection();
            }
        }
    }
    protected virtual void TryAlternateDirections()
    {

        // Try each cardinal direction to see if one is unblocked
        foreach (Vector2 direction in directions)
        {
            if (!IsPathBlocked(direction))
            {
                currentDirection = direction;
                return;
            }
        }

        // If all directions are blocked, we'll just wait
        animator.SetBool("isMoving", false);

    }
    protected virtual void FacePlayer()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float directionX = player.transform.position.x - transform.position.x;
            spriteRenderer.flipX = directionX < 0;
        }

    }

    #endregion

    #region Damage & Death

    public virtual void ApplyDamage(int damage)
    {
        if (isDead || isInDamageState) return; // Ignore if already dead or in damage state

        health -= damage;
        animator.SetTrigger("takeDamage");
        Debug.Log($"Enemy took {damage} damage, remaining health: {health}");
        if (health <= 0 && !isDead)
        {
            isDead = true;
            animator.ResetTrigger("IsAttacking");
            animator.ResetTrigger("takeDamage");
            animator.SetBool("isMoving", false);
            enemyCollider.enabled = false; // Disable collider on death
            animator.SetTrigger("Death");
        }
    }

    public virtual void OnDeathAnimationComplete()
    {
        Die();
    }

    protected virtual void Die()
    {
        StatsManager.Instance.IncrementStat("totalEnemiesKilled");
        GameManager.Instance.HandleEnemyDeath(); // Still let GameManager handle enemy count logic
        Destroy(this.gameObject);
    }

    #endregion

    #region Combat

    protected virtual void Attack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            FacePlayer();
            animator.SetTrigger("IsAttacking");
            lastAttackTime = Time.time;
        }
    }

    public virtual void DealDamageToPlayer()
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

    #endregion

    #region Detection

    protected virtual bool IsPlayerInAttackRange()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
                return true;
        }
        return false;
    }

    protected virtual bool IsPlayerInChaseRange()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, chaseRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
                return true;
        }
        return false;
    }

    protected virtual bool IsPathBlocked(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f);
        return hit.collider != null && hit.collider.CompareTag("Wall");
    }

    protected virtual Vector2 GetRandomCardinalDirection()
    {
        return directions[Random.Range(0, directions.Length)];
    }

    protected virtual Vector2 GetBestCardinalDirection(Vector2 targetDirection)
    {
        float bestDot = -Mathf.Infinity;
        Vector2 bestDirection = Vector2.zero;

        foreach (Vector2 direction in directions)
        {
            float dot = Vector2.Dot(direction.normalized, targetDirection.normalized);
            if (dot > bestDot)
            {
                bestDot = dot;
                bestDirection = direction;
            }
        }

        return bestDirection;
    }

    #endregion

    #region Animation State Hooks

    public virtual void OnAttackStart() => isInAttackState = true;
    public virtual void OnAttackEnd() => isInAttackState = false;
    public virtual void OnDamageStart() => isInDamageState = true;
    public virtual void OnDamageEnd() => isInDamageState = false;

    #endregion

    #region Gizmos

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }

    #endregion

    private void LateUpdate()
    {
        if (useNavMesh && navAgent != null)
        {
         transform.position = navAgent.nextPosition; // Sync position with NavMeshAgent
         navAgent.nextPosition= transform.position; // Ensure NavMeshAgent updates correctly
        }
    }
}
