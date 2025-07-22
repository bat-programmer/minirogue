// Wavy movement effect
using UnityEngine;

public class WavyMovementEffect : FireballEffect
{
    [SerializeField] private float waveAmplitude = 1f;
    [SerializeField] private float waveFrequency = 10f;
    private Vector2 originalDirection;
    private Vector2 basePosition;
    private float timeElapsed = 0f;
    private Vector2 lastPosition;

    public override void ApplyEffect()
    {
        originalDirection = fireball.GetDirection();
        basePosition = fireball.transform.position;
        lastPosition = basePosition;
    }

    public override void UpdateEffect()
    {
        timeElapsed += Time.deltaTime;

        // Create perpendicular vector for wave motion
        Vector2 perpendicular = new Vector2(-originalDirection.y, originalDirection.x);
        float waveOffset = Mathf.Sin(timeElapsed * waveFrequency) * waveAmplitude;

        // Calculate wave force
        Vector2 waveForce = perpendicular * waveOffset * waveFrequency;

        // Apply movement - physics direction remains original, visual gets wavy motion
        fireball.GetComponent<Rigidbody2D>().velocity = originalDirection * fireball.speed + waveForce;

        // Update visual direction for sprite flipping
        Vector2 visualDirection = (originalDirection + waveForce.normalized * 0.3f).normalized;
        fireball.SetDirection(visualDirection);
    }
}

// Speed and damage modifier effect
public class SpeedDamageModifierEffect : FireballEffect
{
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float damageMultiplier = 1f;
    private float originalSpeed;
    private int originalDamage;

    public void SetModifiers(float speedMod, float damageMod)
    {
        speedMultiplier = speedMod;
        damageMultiplier = damageMod;
    }

    public override void ApplyEffect()
    {
        originalSpeed = fireball.speed;
        originalDamage = fireball.damage;

        fireball.speed *= speedMultiplier;
        fireball.damage = Mathf.RoundToInt(fireball.damage * damageMultiplier);
    }

    public override void RemoveEffect()
    {
        fireball.speed = originalSpeed;
        fireball.damage = originalDamage;
        isActive = false;
    }
}

// Tracking effect
public class TrackingEffect : FireballEffect
{
    [SerializeField] private float trackingRange = 10f;
    [SerializeField] private float trackingStrength = 2f;
    private Transform target;

    public override void ApplyEffect()
    {
        FindNearestEnemy();
    }

    public override void UpdateEffect()
    {
        if (target == null)
        {
            FindNearestEnemy();
            return;
        }

        // Calculate direction to target
        Vector2 directionToTarget = (target.position - fireball.transform.position).normalized;
        Vector2 currentDirection = fireball.GetDirection();

        // Blend current direction with target direction
        Vector2 newDirection = Vector2.Lerp(currentDirection, directionToTarget, trackingStrength * Time.deltaTime);
        fireball.SetDirection(newDirection.normalized);
    }

    private void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = trackingRange;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(fireball.transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                target = enemy.transform;
            }
        }
    }
}

// Bounce effect
public class BounceEffect : FireballEffect
{
    [SerializeField] private int maxBounces = 3;
    private int currentBounces = 0;

    public override void ApplyEffect()
    {
        // Modify the fireball's collision behavior
        fireball.OnWallHit += HandleBounce;
    }

    private void HandleBounce(Vector2 hitNormal)
    {
        if (currentBounces < maxBounces)
        {
            Vector2 currentDirection = fireball.GetDirection();
            Vector2 bounceDirection = Vector2.Reflect(currentDirection, hitNormal);
            fireball.SetDirection(bounceDirection);
            currentBounces++;
        }
        else
        {
            // No more bounces left, disable the fireball
            fireball.DisableFireball();
        }
    }

    public override void RemoveEffect()
    {
        fireball.OnWallHit -= HandleBounce;
        isActive = false;
    }
}


// Double fire effect - splits fireball into two
// Double fire effect - splits fireball into two
public class DoubleFireEffect : FireballEffect
{
    [SerializeField] private float splitAngle = 30f; // Angle between the two fireballs
    [SerializeField] private float splitDelay = 0.5f; // Time before splitting
    [SerializeField] private bool splitOnWallHit = false; // Alternative trigger

    private bool hasTriggered = false;
    private float timeElapsed = 0f;

    public override void ApplyEffect()
    {
        if (splitOnWallHit)
        {
            fireball.OnWallHit += HandleWallHitSplit;
        }
    }

    public override void UpdateEffect()
    {
        if (!hasTriggered && !splitOnWallHit)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= splitDelay)
            {
                TriggerSplit();
            }
        }
    }

    private void HandleWallHitSplit(Vector2 hitNormal)
    {
        if (!hasTriggered)
        {
            TriggerSplit();
        }
    }

    private void TriggerSplit()
    {
        hasTriggered = true;

        // Get current fireball state
        Vector2 currentDirection = fireball.GetDirection();
        Vector2 currentPosition = fireball.transform.position;

        // Calculate split directions
        float angleRad = splitAngle * Mathf.Deg2Rad;
        Vector2 leftDirection = RotateVector(currentDirection, angleRad);
        Vector2 rightDirection = RotateVector(currentDirection, -angleRad);

        // Create second fireball
        GameObject secondFireball = CreateFireballCopy(rightDirection);
        if (secondFireball != null)
        {
            BolaDeFuego secondFireballScript = secondFireball.GetComponent<BolaDeFuego>();

            // Copy all effects except DoubleFireEffect to prevent infinite splitting
            CopyEffectsToFireball(secondFireballScript);
        }

        // Update original fireball direction
        fireball.SetDirection(leftDirection);
    }

    private Vector2 RotateVector(Vector2 vector, float angleRad)
    {
        float cos = Mathf.Cos(angleRad);
        float sin = Mathf.Sin(angleRad);
        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos
        );
    }

    private GameObject CreateFireballCopy(Vector2 direction)
    {
        // Try to get from pool first
        PoolBolaDeFuego pool = FindObjectOfType<PoolBolaDeFuego>();
        if (pool != null)
        {
            return pool.GetFireball(fireball.transform.position, direction, fireball.damage);
        }
        else
        {
            // Fallback to instantiation
            GameObject newFireball = Instantiate(fireball.gameObject, fireball.transform.position, Quaternion.identity);
            BolaDeFuego newFireballScript = newFireball.GetComponent<BolaDeFuego>();
            newFireballScript.SetDirection(direction);
            newFireballScript.SetDamage(fireball.damage);
            return newFireball;
        }
    }

    private void CopyEffectsToFireball(BolaDeFuego targetFireball)
    {
        // Copy each active effect except DoubleFireEffect
        foreach (var effect in fireball.GetComponents<FireballEffect>())
        {
            if (effect is DoubleFireEffect) continue; // Skip to prevent infinite splitting

            // Get the remaining duration from the current effect
            float remainingDuration = effect.RemainingDuration;

            // Copy the effect based on type
            if (effect is WavyMovementEffect)
            {
                targetFireball.AddEffect<WavyMovementEffect>(remainingDuration);
            }
            else if (effect is TrackingEffect)
            {
                targetFireball.AddEffect<TrackingEffect>(remainingDuration);
            }
            else if (effect is BounceEffect)
            {
                targetFireball.AddEffect<BounceEffect>(remainingDuration);
            }
            else if (effect is SpeedDamageModifierEffect speedDamage)
            {
                var newEffect = targetFireball.gameObject.AddComponent<SpeedDamageModifierEffect>();
                newEffect.Initialize(targetFireball, remainingDuration);
                newEffect.ApplyEffect();
            }
        }
    }

    public override void RemoveEffect()
    {
        if (splitOnWallHit)
        {
            fireball.OnWallHit -= HandleWallHitSplit;
        }
        isActive = false;
    }
}
