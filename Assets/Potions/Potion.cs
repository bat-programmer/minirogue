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
            if (GameManager.Instance.potionManager.currentPotion == null)
            {                
                IPotionEffect effect = GetComponent<IPotionEffect>();
                if (effect == null)
                {
                    Debug.LogError("Potion has no IPotionEffect component assigned. Cannot equip.");
                    return; // Exit if no effect is found
                }

                RandomPotionColor randomPotionColor = GetComponentInChildren<RandomPotionColor>();
                if (randomPotionColor == null)
                {
                    Debug.LogError("Potion has no RandomPotionColor component assigned. Cannot equip.");
                    return; // Exit if no RandomPotionColor is found
                }

                SpriteRenderer sr = randomPotionColor.liquidRenderer;
                if (sr == null)
                {
                    Debug.LogError("RandomPotionColor's liquidRenderer is null. Cannot equip.");
                    return; // Exit if liquidRenderer is null
                }
                
                // Show "???" for unidentified potions
                string label = GameManager.Instance.potionManager.IsPotionDiscovered(effect.GetType()) 
                    ? effect.GetUILabel() 
                    : "???";
                    
                GameManager.Instance.EquipPotion(effect, label, sr.sprite, sr.color);
                Destroy(gameObject);
            }
        }
    }
}
