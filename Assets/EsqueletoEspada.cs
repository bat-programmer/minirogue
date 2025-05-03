using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EsqueletoEspada : MonoBehaviour
{
    private Animator animator; // Reference to Animator
    private SpriteRenderer spriteRenderer;
    private Vector2 direction;
    public int vida_enemigo = 100;

    private float wanderInterval = 3.0f;
    private float wanderTimer = 0.0f;
    public float attackRange = 1.5f; // Range to detect the player
    private float attackCooldown = 1.0f;
    private float lastAttackTime = -Mathf.Infinity;
    
    private float speed = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        direction = GetRandomDirection();
        animator = GetComponent<Animator>(); // Get the Animator component
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlayerNearby())
        {
            Attack();
        }
        else
        {
            //Wander();
        }
    }
    private Vector2 GetRandomDirection()
    {
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    public void TakeDamage()
    {
        vida_enemigo -= 10; 
        animator.SetTrigger("takeDamage");
        Debug.Log("Enemy hit! Remaining health: " + vida_enemigo);
    }

    private void Wander()
    {
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= wanderInterval)
        {
            direction = GetRandomDirection();
            wanderTimer = 0.0f;
        }

        transform.Translate(direction * speed * Time.deltaTime);

        // **Flip sprite based on direction**
        if (direction.x > 0)
        {
            spriteRenderer.flipX = false; // Facing right
        }
        else if (direction.x < 0)
        {
            spriteRenderer.flipX = true; // Facing left
        }

        // **Set Animation**
        animator.SetBool("isMoving", direction.magnitude > 0);

        // Check for collision with walls
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.1f);
        if (hit.collider != null && hit.collider.CompareTag("Wall"))
        {
            direction = GetRandomDirection();
        }
    }
   private bool IsPlayerNearby()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Player is nearby!");
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
    }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the attack range in the editor for visualization
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

private void FacePlayer()
{
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player != null)
    {
        float directionX = player.transform.position.x - transform.position.x;
        if (directionX > 0)
        {
            spriteRenderer.flipX = false; // Face right
        }
        else if (directionX < 0)
        {
            spriteRenderer.flipX = true; // Face left
        }
    }
}


}
