using UnityEngine;

public class WandPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private GameObject pickupEffect;
    [SerializeField] private float rotationSpeed = 60f;

    [Header("Wand Configuration")]
    [SerializeField] private WandEffectData wandData; // This is where you assign your ScriptableObject

    // Optional: Override settings (if you want to customize individual instances)
    [SerializeField] private bool useOverrides = false;
    [SerializeField] private FireballEffectType overrideEffectType;
    [SerializeField] private WandDurationType overrideDurationType;
    [SerializeField] private float overrideTemporalDuration;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Apply visual settings from ScriptableObject
        if (wandData && spriteRenderer)
        {
            if (wandData.wandSprite)
                spriteRenderer.sprite = wandData.wandSprite;
            spriteRenderer.color = wandData.wandColor;
        }
    }

    private void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Jugador>();
        if (player != null)
        {
            if (wandData != null)
            {
                FireballEffectType effectToApply = useOverrides ? overrideEffectType : wandData.effectType;
                float duration = GetDuration();

                player.GetComponent<PlayerAttackController>().AddFireballEffect(effectToApply, duration);

                // Show pickup message
                Debug.Log(GetPickupMessage());

                if (pickupEffect)
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);

                // Notify GameManager about the picked up wand
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.RegisterPickedUpWand(wandData.effectType);
                }

                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("WandPickup: No WandEffectData assigned!");
            }
        }
    }

    private float GetDuration()
    {
        if (useOverrides)
        {
            return overrideDurationType == WandDurationType.Permanent ? -1f : overrideTemporalDuration;
        }
        return wandData.GetDuration();
    }

    private string GetPickupMessage()
    {
        if (useOverrides)
        {
            string effectName = overrideEffectType.ToString();
            if (overrideDurationType == WandDurationType.Permanent)
                return $"Permanent {effectName} ability gained!";
            else
                return $"Temporary {effectName} boost for {overrideTemporalDuration} seconds!";
        }
        return wandData.GetPickupMessage();
    }
}

public enum WandEffectMode
{
    Specific,
    Random
}

public enum WandDurationType
{
    Permanent,
    Temporal
}

public enum FireballEffectType
{
    WavyMovement,
    Tracking,
    Bounce,
    DoubleFireEffect
}
