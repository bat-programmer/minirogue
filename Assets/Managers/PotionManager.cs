using Assets.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // Assuming FloatingText uses TextMeshPro

public class PotionManager : MonoBehaviour
{
    public static PotionManager Instance { get; private set; }

    public IPotionEffect currentPotion;
    public PotionUISlot potionUISlot; // Assign in the Inspector
    public AudioSource audioSource; // Assign in the Inspector

    // Track discovered potion types
    private Dictionary<System.Type, bool> discoveredPotions = new Dictionary<System.Type, bool>();

    // Track first potion assignment
    public bool HasAssignedFirstPotion { get; set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool IsPotionDiscovered(System.Type potionType)
    {
        return discoveredPotions.ContainsKey(potionType);
    }

    public void DiscoverPotion(IPotionEffect effect)
    {
        discoveredPotions[effect.GetType()] = true;
    }

    public void EquipPotion(IPotionEffect effect, string label, Sprite sprite, Color color, Transform playerTransform)
    {
        Vector3 textPosition = playerTransform != null ? playerTransform.position : transform.position;

        FloatingText.Create(textPosition, "Potion Grabbed!", Color.green);
        Debug.Log("Potion equipped: " + effect.GetType().Name);
        currentPotion = effect;
        Debug.Log("Potion color: " + color.ToString());
        if (potionUISlot != null)
        {
            potionUISlot.SetPotion(sprite, color, label);
        }
        else
        {
            Debug.LogError("PotionUISlot is not assigned in PotionManager. Please assign it in the Inspector.");
        }

        // Play pickup sound
        if (audioSource != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource not set in PotionManager.");
        }

        // Mark as discovered when equipped
        DiscoverPotion(effect);
    }

    public void UsePotion(Jugador player)
    {
        if (currentPotion != null)
        {
            string potionName = currentPotion.GetUILabel();
            currentPotion.ApplyEffect(player);
            currentPotion = null;
            if (potionUISlot != null)
            {
                potionUISlot.ClearPotion();
            }
            else
            {
                Debug.LogError("PotionUISlot is not assigned in PotionManager. Please assign it in the Inspector.");
            }
            FloatingText.Create(player.transform.position, $"{potionName} used", Color.yellow);
            // Assuming StatsManager is still globally accessible
            StatsManager.Instance.IncrementStat("potionsUsed");
        }
    }
}
