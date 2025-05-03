using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EsqueletoEspada : MonoBehaviour
{
    // Inspector properties
    public float speed = 3.0f;
    public int vida_enemigo = 100;
    public float attackRange = 1.5f;
    public float chaseRange = 8.0f;    // Range to detect and chase player
    public float pathUpdateInterval = 0.5f; // How often to recalculate path to player

    // Movement constraints
    private readonly Vector2[] directions = new Vector2[]
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };

    // Private fields
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 currentDirection = Vector2.zero;
    private float pathUpdateTimer = 0f;
    private float wanderTimer = 0.0f;
    private float wanderInterval = 3.0f;
    private float attackCooldown = 1.0f;
    private float lastAttackTime = -Mathf.Infinity;

    // State tracking
    private bool isInAttackState = false;
    private bool isInDamageState = false;
    private bool isChasing = false;

    void Start()
    {
        currentDirection = GetRandomCardinalDirection();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Only perform behaviors if not in a blocking state
        if (!isInAttackState && !isInDamageState)
        {
            if (IsPlayerInAttackRange())
            {
                Attack();
            }
            else if (IsPlayerInChaseRange())
            {
                ChasePlayer();
            }
            else
            {
                Wander();
            }
        }
    }

    // Called from AttackStateBehavior script
    public void OnAttackStart()
    {
        isInAttackState = true;
    }

    public void OnAttackEnd()
    {
        isInAttackState = false;
    }

    public void OnDamageStart()
    {
        isInDamageState = true;
    }

    public void OnDamageEnd()
    {
        isInDamageState = false;
    }

    private Vector2 GetRandomCardinalDirection()
    {
        // Get a random cardinal direction (up, down, left, right)
        return directions[Random.Range(0, directions.Length)];
    }

    public void ApplyDamage()
    {
        vida_enemigo -= 10;
        animator.SetTrigger("takeDamage");
        Debug.Log("Enemy hit! Remaining health: " + vida_enemigo);

        // Check for death
        if (vida_enemigo <= 0)
        {
            // Handle death
            animator.SetTrigger("death");
            // Disable components
            GetComponent<Collider2D>().enabled = false;
            this.enabled = false;
        }
    }

    private void Wander()
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

    private void ChasePlayer()
    {
        isChasing = true;

        // Update path to player periodically
        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= pathUpdateInterval)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Calculate direction to player
                Vector2 directionToPlayer = player.transform.position - transform.position;

                // Determine which cardinal direction gets us closest to the player
                currentDirection = GetBestCardinalDirection(directionToPlayer);
            }
            pathUpdateTimer = 0f;
        }

        MoveInDirection(currentDirection);
    }

    private Vector2 GetBestCardinalDirection(Vector2 targetDirection)
    {
        // Determine which cardinal direction is closest to the target direction
        float bestDot = -Mathf.Infinity;
        Vector2 bestDirection = Vector2.zero;

        foreach (Vector2 direction in directions)
        {
            // Calculate dot product (how aligned the directions are)
            float dot = Vector2.Dot(direction.normalized, targetDirection.normalized);

            // If this direction is better, update our best direction
            if (dot > bestDot)
            {
                bestDot = dot;
                bestDirection = direction;
            }
        }

        return bestDirection;
    }

    private void MoveInDirection(Vector2 direction)
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

    private void TryAlternateDirections()
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

    private bool IsPathBlocked(Vector2 direction)
    {
        // Cast a ray to check if there's an obstacle in the direction we want to move
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f);

        // Check if we hit something that's not the player
        if (hit.collider != null && !hit.collider.CompareTag("Player"))
        {
            // We hit something - check if it's a wall or obstacle
            if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Obstacle"))
            {
                return true; // Path is blocked
            }
        }

        return false; // Path is clear
    }

    private bool IsPlayerInAttackRange()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsPlayerInChaseRange()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, chaseRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    private void Attack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            FacePlayer();
            animator.SetTrigger("IsAttacking");
            lastAttackTime = Time.time;
            Debug.Log("Enemy is attacking!");

            // Animation events will handle the rest
        }
    }

    // This can be called from an Animation Event to deal damage at the right moment
    public void DealDamageToPlayer()
    {
        // Find player in attack range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Jugador jugador = hit.GetComponent<Jugador>();
                if (jugador != null)
                {
                    jugador.ApplyDamage(10); // Deal damage to player
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the attack range in the editor for visualization
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw the chase range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }

    private void FacePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float directionX = player.transform.position.x - transform.position.x;
            spriteRenderer.flipX = directionX < 0;
        }
    }
}