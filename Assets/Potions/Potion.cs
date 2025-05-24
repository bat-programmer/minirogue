using Assets.Interfaces;
using UnityEngine;

public class Potion : MonoBehaviour
{
    private IPotionEffect potionEffect;

    private void Start()
    {
        // Get the potion effect from this GameObject
        potionEffect = GetComponent<IPotionEffect>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Jugador>(out Jugador player))
        {

            potionEffect?.ApplyEffect(player); // Apply effect if assigned
            Destroy(gameObject); // Remove potion after use
        }
    }
}
