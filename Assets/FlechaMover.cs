using UnityEngine;

public class FlechaMover : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }

    //if it collides with something, apply damage and destroy itself
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Assuming the player has a method to take damage
            EnemyBase enemy = collision.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.ApplyDamage(50); // Adjust damage value as needed
            }

        }
        // Destroy the arrow after it hits something
        
        if (collision.CompareTag("Player"))
        {
            // Assuming the player has a method to take damage
            Jugador jugador = collision.GetComponent<Jugador>();
            if (jugador != null)
            {
                jugador.ApplyDamage(50); // Adjust damage value as needed
            }
        }

        if (collision.CompareTag("Wall"))
        {
            // If it hits a wall, just destroy itself
            Destroy(gameObject);
            return;
        }
        //Destroy(gameObject);
    }
}