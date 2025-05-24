using Assets.Interfaces;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public IPotionEffect currentPotion;
    public PotionUISlot potionUISlot; // Assign in the Inspector
    public static GameManager Instance;

    public int playerHealth = 100;

    // Reference to the current potion the player is holding
    public GameObject heldPotion;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void EquipPotion(IPotionEffect effect, Sprite sprite, Color color)
    {
        Debug.Log("Potion equipped: " + effect.GetType().Name);
        currentPotion = effect;
        //log potion color
        Debug.Log("Potion color: " + color.ToString());
        potionUISlot.SetPotion(sprite, color);
    }

    public void UsePotion(Jugador player)
    {
        if (currentPotion != null)
        {
            currentPotion.ApplyEffect(player);
            currentPotion = null;
            potionUISlot.ClearPotion();
        }
    }
}
