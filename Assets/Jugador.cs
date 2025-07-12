using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Jugador : MonoBehaviour
{
    private MovementController movementController;
    private Animator anim;
    private SpriteRenderer sr;
    public Transform firePoint;
    [SerializeField] private float speed = 100f;
    [SerializeField] private int fireballDamage = 1000; // Default damage
    private Coroutine fireballDamageBoostCoroutine;

    [Header("UI")]
    public Transform heartsContainer; // Drag your container here in the Inspector

    [Header("Health System")]
    public List<int> hearts = new List<int>(); // Each heart can have values: 2 (full), 1 (half), 0 (empty)
    public int maxHearts = 3; // Maximum number of hearts

    private bool isInvulnerable = false;
    [SerializeField] private float invulnerabilityDuration = 5f;
    [SerializeField] private float blinkInterval = 0.2f;
    private Vector2 lastDirection = Vector2.right; // Default direction
    private PoolBolaDeFuego poolBolaDeFuego;

    [Header("Attack Settings")]
    [SerializeField] public float fireRate = 0.1f; // Seconds between shots
    public float fireCooldown = 1f;
    private Vector2? heldFireDirection = null;

    [Header("Wand Effects")]
    [SerializeField] private List<FireballEffectType> permanentEffects = new List<FireballEffectType>();
    [SerializeField] private List<TemporalEffect> temporalEffects = new List<TemporalEffect>();

    [Header("Events")]
    public System.Action OnDamageTaken;

    public void AddFireballEffect(FireballEffectType effectType, float duration = -1f)
    {
        if (duration <= 0) // Permanent effect
        {
            if (!permanentEffects.Contains(effectType))
            {
                permanentEffects.Add(effectType);
                Debug.Log($"Permanent wand effect {effectType} added!");
            }
            else
            {
                Debug.Log($"Permanent effect {effectType} already exists!");
            }
        }
        else // Temporal effect
        {
            // Check if we already have this temporal effect and refresh its duration
            var existingEffect = temporalEffects.Find(e => e.effectType == effectType);
            if (existingEffect != null)
            {
                existingEffect.duration = duration; // Refresh duration
                Debug.Log($"Temporal wand effect {effectType} duration refreshed to {duration} seconds!");
            }
            else
            {
                temporalEffects.Add(new TemporalEffect(effectType, duration));
                Debug.Log($"Temporal wand effect {effectType} applied for {duration} seconds!");
            }
        }
    }

    public void RemovePermanentEffect(FireballEffectType effectType)
    {
        if (permanentEffects.Remove(effectType))
        {
            Debug.Log($"Permanent effect {effectType} removed!");
        }
    }

    public void ClearAllPermanentEffects()
    {
        permanentEffects.Clear();
        Debug.Log("All permanent effects cleared!");
    }

    private void ApplyFireballEffects(BolaDeFuego fireball)
    {
        // Apply all permanent effects
        foreach (var effectType in permanentEffects)
        {
            ApplySpecificEffect(fireball, effectType);
        }

        // Apply all active temporal effects
        foreach (var temporalEffect in temporalEffects)
        {
            ApplySpecificEffect(fireball, temporalEffect.effectType);
        }
    }
    private void ApplySpecificEffect(BolaDeFuego fireball, FireballEffectType effectType)
    {
        switch (effectType)
        {
            case FireballEffectType.WavyMovement:
                fireball.AddEffect<WavyMovementEffect>();
                break;
            case FireballEffectType.Tracking:
                fireball.AddEffect<TrackingEffect>();
                break;
            case FireballEffectType.Bounce:
                fireball.AddEffect<BounceEffect>();
                break;
            case FireballEffectType.DoubleFireEffect:
                fireball.AddEffect<DoubleFireEffect>();
                break;
        }
    }
    private void UpdateTemporalEffects()
    {
        for (int i = temporalEffects.Count - 1; i >= 0; i--)
        {
            temporalEffects[i].duration -= Time.deltaTime;
            if (temporalEffects[i].duration <= 0)
            {
                Debug.Log($"Temporal effect {temporalEffects[i].effectType} expired!");
                temporalEffects.RemoveAt(i);
            }
        }
    }
    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        poolBolaDeFuego = FindObjectOfType<PoolBolaDeFuego>();

        movementController = gameObject.AddComponent<MovementController>();
        movementController.Initialize(GetComponent<Rigidbody2D>());
        movementController.Speed = speed;

        InitializeHearts(maxHearts);
        UpdateHeartUI();
        GameManager.Instance.playerTransform= transform; // Set player transform in GameManager
    }

    void Update()
    {
        movementController.UpdateMovement();
        HandleAttackInput();
        usePotion();
        UpdateTemporalEffects();
    }

    private KeyCode GetKeyBinding(string prefKey, KeyCode defaultKey)
    {
        if (PlayerPrefs.HasKey(prefKey))
            return (KeyCode)PlayerPrefs.GetInt(prefKey);
        return defaultKey;
    }

    private void HandleAttackInput()
    {
        KeyCode upKey = GetKeyBinding("AttackUp", KeyCode.UpArrow);
        KeyCode downKey = GetKeyBinding("AttackDown", KeyCode.DownArrow);
        KeyCode leftKey = GetKeyBinding("AttackLeft", KeyCode.LeftArrow);
        KeyCode rightKey = GetKeyBinding("AttackRight", KeyCode.RightArrow);

        // Detect if any attack key is held and set the direction
        if (Input.GetKey(upKey))
            heldFireDirection = Vector2.up;
        else if (Input.GetKey(downKey))
            heldFireDirection = Vector2.down;
        else if (Input.GetKey(leftKey))
            heldFireDirection = Vector2.left;
        else if (Input.GetKey(rightKey))
            heldFireDirection = Vector2.right;
        else
            heldFireDirection = null;

        // Fire if holding a direction and cooldown is over
        if (heldFireDirection.HasValue)
        {
            if (fireCooldown <= 0f)
            {
                anim.SetTrigger("Attack");
                Fire(heldFireDirection.Value);
                fireCooldown = fireRate;
            }
        }

        // Reduce cooldown timer
        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;
    }

    private void usePotion()
    {
        KeyCode potionKey = GetKeyBinding("UsePotion", KeyCode.E);
        if (Input.GetKeyDown(potionKey))
        {
            GameManager.Instance.UsePotion(this);
        }
    }

    private void Fire(Vector2 direction)
    {
        if (poolBolaDeFuego != null)
        {
            GameObject fireballObj = poolBolaDeFuego.GetFireball(firePoint.position, direction.normalized, fireballDamage);
            BolaDeFuego fireball = fireballObj.GetComponent<BolaDeFuego>();
            ApplyFireballEffects(fireball);
        }
    }

    public void ApplyFireballDamageBoost(int amount, float duration)
    {
        if (fireballDamageBoostCoroutine != null)
            StopCoroutine(fireballDamageBoostCoroutine);

        fireballDamageBoostCoroutine = StartCoroutine(FireballDamageBoostCoroutine(amount, duration));
    }

    private IEnumerator FireballDamageBoostCoroutine(int amount, float duration)
    {
        fireballDamage += amount;
        Debug.Log($"Fireball damage increased by {amount} for {duration} seconds.");

        yield return new WaitForSeconds(duration);

        fireballDamage -= amount;
        Debug.Log("Fireball damage boost ended.");
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

    public void AddMoney(int amount)
    {
        GameManager.Instance.AddCoins(amount);
    }

    public void RemoveMoney(int amount)
    {
        GameManager.Instance.RemoveCoins(amount);
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

    public List<FireballEffectType> GetPermanentEffects()
    {
        return new List<FireballEffectType>(permanentEffects);
    }

    public List<TemporalEffect> GetTemporalEffects()
    {
        return new List<TemporalEffect>(temporalEffects);
    }

    public bool HasEffect(FireballEffectType effectType)
    {
        return permanentEffects.Contains(effectType) ||
               temporalEffects.Exists(e => e.effectType == effectType);
    }
}
