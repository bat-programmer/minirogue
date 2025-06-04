using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 direction;

    public void Initialize(Vector2 direction)
    {
        this.direction = direction.normalized;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Apply damage if needed
            collision.GetComponent<Jugador>()?.ApplyDamage(1);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
