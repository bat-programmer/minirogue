using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health System")]
    public List<int> hearts = new List<int>(); // Each heart can have values: 2 (full), 1 (half), 0 (empty)
    public int maxHearts = 3; // Maximum number of hearts

    private bool isInvulnerable = false;
    [SerializeField] private float invulnerabilityDuration = 2f;
    [SerializeField] private float blinkInterval = 0.2f;

    [Header("UI")]
    public Transform heartsContainer; // Drag your container here in the Inspector

    private SpriteRenderer sr;
    public event System.Action OnDamageTaken;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        InitializeHearts(maxHearts);
        UpdateHeartUI();
    }

    // Initialize hearts with full health
    private void InitializeHearts(int count)
    {
        hearts.Clear();
        for (int i = 0; i < count; i++)
        {
            hearts.Add(2); // Full heart
        }
    }

    // Add health (1 = half heart, 2 = full heart)
    public void AddHealth(int amount)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (hearts[i] < 2)
            {
                hearts[i] += amount;
                if (hearts[i] > 2) hearts[i] = 2; // Cap at full heart
                amount -= 1;
                if (amount <= 0) break;
            }
        }

        UpdateHeartUI();
        Debug.Log("Health added. Current hearts: " + string.Join(", ", hearts));
    }

    // Remove health (1 = half heart, 2 = full heart)
    public void RemoveHealth(int amount)
    {
        for (int i = hearts.Count - 1; i >= 0; i--)
        {
            if (hearts[i] > 0)
            {
                hearts[i] -= amount;
                if (hearts[i] < 0) hearts[i] = 0; // Cap at empty heart
                amount -= 1;
                if (amount <= 0) break;
            }
        }
        UpdateHeartUI();
        Debug.Log("Health removed. Current hearts: " + string.Join(", ", hearts));
    }

    public void ApplyHealth(int amount)
    {
        AddHealth(amount);
        if (IsDead())
        {
            // Handle player death (e.g., restart level, show game over screen)
            Debug.Log("Player is dead!");
        }
    }

    public void ApplyDamage(int damage)
    {
        if (isInvulnerable) return;

        RemoveHealth(damage);
        OnDamageTaken?.Invoke();
        StatsManager.Instance.IncrementStat("damageTaken", damage);

        if (IsDead())
        {
            TriggerDeathEffect();
        }
        else
        {
            StartCoroutine(InvulnerabilityCoroutine());
        }
    }

    private void TriggerDeathEffect()
    {
        // Disable player components
        GetComponent<Collider2D>().enabled = false;
        enabled = false; // Disable this script

        // Trigger death effect
        PlayerDeathEffect deathEffect = gameObject.AddComponent<PlayerDeathEffect>();
        deathEffect.TriggerDeathEffect(GetComponent<SpriteRenderer>());

        // Show defeat screen
        GameManager.Instance.ShowDefeatScreen();
    }

    public bool IsDead()
    {
        foreach (int heart in hearts)
        {
            if (heart > 0)
                return false; // Player is alive
        }
        return true; // Player is dead
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;

        float elapsed = 0f;
        while (elapsed < invulnerabilityDuration)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        sr.enabled = true; // Make sure sprite is visible at end
        isInvulnerable = false;
    }

    void UpdateHeartUI()
    {
        for (int i = 0; i < heartsContainer.childCount; i++)
        {
            var heartUI = heartsContainer.GetChild(i).GetComponent<HeartImage>();

            if (i < hearts.Count)
            {
                heartUI.SetState(hearts[i]);
            }
            else
            {
                heartUI.gameObject.SetActive(false); // Optional: hide extra hearts
            }
        }
    }
}
