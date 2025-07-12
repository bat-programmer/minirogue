using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    private Animator anim;
    public Transform firePoint;
    public float fireCooldown = 1f;
    private Vector2? heldFireDirection = null;
    private PoolBolaDeFuego poolBolaDeFuego;
    private int fireballDamage = 1000;

    [Header("Wand Effects")]
    [SerializeField] private List<FireballEffectType> permanentEffects = new List<FireballEffectType>();
    [SerializeField] private List<TemporalEffect> temporalEffects = new List<TemporalEffect>();

    [Header("Attack Settings")]
    [SerializeField] public float fireRate = 0.1f; // Seconds between shots

    public void Initialize(Animator animator, Transform firePointTransform, PoolBolaDeFuego pool)
    {
        anim = animator;
        firePoint = firePointTransform;
        poolBolaDeFuego = pool;
    }

    public void AddFireballEffect(FireballEffectType effectType, float duration = -1f)
    {
        if (duration <= 0)
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
        else
        {
            var existingEffect = temporalEffects.Find(e => e.effectType == effectType);
            if (existingEffect != null)
            {
                existingEffect.duration = duration;
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
        foreach (var effectType in permanentEffects)
        {
            ApplySpecificEffect(fireball, effectType);
        }

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

    public void HandleAttackInput()
    {
        KeyCode upKey = GetKeyBinding("AttackUp", KeyCode.UpArrow);
        KeyCode downKey = GetKeyBinding("AttackDown", KeyCode.DownArrow);
        KeyCode leftKey = GetKeyBinding("AttackLeft", KeyCode.LeftArrow);
        KeyCode rightKey = GetKeyBinding("AttackRight", KeyCode.RightArrow);

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

        if (heldFireDirection.HasValue)
        {
            if (fireCooldown <= 0f)
            {
                anim.SetTrigger("Attack");
                Fire(heldFireDirection.Value);
                fireCooldown = fireRate;
            }
        }

        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;
    }

    private KeyCode GetKeyBinding(string prefKey, KeyCode defaultKey)
    {
        if (PlayerPrefs.HasKey(prefKey))
            return (KeyCode)PlayerPrefs.GetInt(prefKey);
        return defaultKey;
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
        StartCoroutine(FireballDamageBoostCoroutine(amount, duration));
    }

    private IEnumerator FireballDamageBoostCoroutine(int amount, float duration)
    {
        fireballDamage += amount;
        Debug.Log($"Fireball damage increased by {amount} for {duration} seconds.");

        yield return new WaitForSeconds(duration);

        fireballDamage -= amount;
        Debug.Log("Fireball damage boost ended.");
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

    void Update()
    {
        UpdateTemporalEffects();
    }
}
