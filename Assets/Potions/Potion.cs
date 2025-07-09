using Assets.Interfaces;
using UnityEngine;

public class Potion : MonoBehaviour
{
    private IPotionEffect potionEffect;

    private void Start()
    {
        potionEffect = GetComponent<IPotionEffect>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Jugador>(out Jugador player))
        {
            if (GameManager.Instance.currentPotion == null)
            {                
                IPotionEffect effect = GetComponent<IPotionEffect>();
                RandomPotionColor randomPotionColor = GetComponentInChildren<RandomPotionColor>();
                SpriteRenderer sr = randomPotionColor.liquidRenderer;
                
                // Show "???" for unidentified potions
                string label = GameManager.Instance.IsPotionDiscovered(effect.GetType()) 
                    ? effect.GetUILabel() 
                    : "???";
                    
                GameManager.Instance.EquipPotion(effect, label, sr.sprite, sr.color);
                Destroy(gameObject);
            }
        }
    }
}
