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
            if (GameManager.Instance.currentPotion == null)
            {
                Debug.Log("Potion picked up: " + gameObject.name);
                IPotionEffect effect = GetComponent<IPotionEffect>();                
                RandomPotionColor randomPotionColor = GetComponentInChildren<RandomPotionColor>();
                SpriteRenderer sr = randomPotionColor.liquidRenderer;
                Debug.Log("grabbed color: " + sr.color.ToString());
                GameManager.Instance.EquipPotion(effect, sr.sprite, sr.color);
                //gameObject.SetActive(false); // Deactivate the potion object instead of destroying it
                Destroy(gameObject);
            }
        }
    }

}
