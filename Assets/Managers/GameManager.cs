using Assets.Interfaces;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public IPotionEffect currentPotion;
    public PotionUISlot potionUISlot; // Assign in the Inspector
    public static GameManager Instance;
    public Transform playerTransform;

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
        Vector3 textPosition = playerTransform != null ? playerTransform.position : transform.position;

        FloatingText.Create(textPosition, "Potion Grabbed!", Color.green);
        Debug.Log("Potion equipped: " + effect.GetType().Name);
        currentPotion = effect;
        //log potion color
        Debug.Log("Potion color: " + color.ToString());
        potionUISlot.SetPotion(sprite, color,effect.GetUILabel());
    }

    public void UsePotion(Jugador player)
    {
        if (currentPotion != null)
        {
            currentPotion.ApplyEffect(player);
            currentPotion = null;
            potionUISlot.ClearPotion();
            FloatingText.Create(player.transform.position, "Potion used", Color.yellow);
        }
    }
}
