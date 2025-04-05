using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MonoBehaviour
{
    public int vida_enemigo = 100;
    private bool berserkOn = false;
    private float berserkSpeed = 10;

    [SerializeField]
    private ClassTimer berserkTimer;

    private float wanderInterval = 3.0f;
    private float wanderTimer = 0.0f;
    private Vector2 direction;
    private float speed = 2.0f;
    


    public GameObject impactEffect;  // Prefab for the impact effect
    private Animator animator; // Reference to Animator
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        direction = GetRandomDirection();
        animator = GetComponent<Animator>(); // Get the Animator component
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (berserkTimer.timerRestante <= 0)
        {
            berserkOn = true;
        }

        if (berserkOn)
        {
            vida_enemigo -= 1;
        }

        Wander();
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

    private Vector2 GetRandomDirection()
    {
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fireball"))
        {
            vida_enemigo -= 10;
            Destroy(collision.gameObject);
            ApplyImpactEffect(collision.transform.position);

            // **Trigger Damage Animation**
            animator.SetTrigger("takeDamage");


        }
    }

    void ApplyImpactEffect(Vector2 position)
    {
        if (impactEffect != null)
        {
            GameObject effect = Instantiate(impactEffect, position, Quaternion.identity);
            Destroy(effect.gameObject, 1f); // Destroy after 1 second
        }
    }
}
