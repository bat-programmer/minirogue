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
    private Vector2 lastDirection = Vector2.right; // Default direction
    private PoolBolaDeFuego poolBolaDeFuego;
    private PlayerHealth playerHealth;

    [Header("UI")]
    public Transform heartsContainer; // Drag your container here in the Inspector

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

        playerHealth = gameObject.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            playerHealth = gameObject.AddComponent<PlayerHealth>();
            playerHealth.heartsContainer = heartsContainer;
            playerHealth.maxHearts = 3;
        }
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

    public void AddMoney(int amount)
    {
        GameManager.Instance.AddCoins(amount);
    }

    public void RemoveMoney(int amount)
    {
        GameManager.Instance.RemoveCoins(amount);
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

    // Health management methods - delegate to PlayerHealth
    public void ApplyDamage(int damage)
    {
        if (playerHealth != null)
        {
            playerHealth.ApplyDamage(damage);
        }
    }

    public void ApplyHealth(int amount)
    {
        if (playerHealth != null)
        {
            playerHealth.ApplyHealth(amount);
        }
    }

    public bool IsDead()
    {
        return playerHealth != null && playerHealth.IsDead();
    }
}
