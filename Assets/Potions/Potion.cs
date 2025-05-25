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
                IPotionEffect effect = GetComponent<IPotionEffect>();
                //GetComponent<MonoBehaviour>() as IPotionEffect;
                RandomPotionColor randomPotionColor = GetComponentInChildren<RandomPotionColor>();
                SpriteRenderer sr = randomPotionColor.liquidRenderer;             
                GameManager.Instance.EquipPotion(effect, sr.sprite, sr.color);                
                Destroy(gameObject);
            }
        }
    }

}
